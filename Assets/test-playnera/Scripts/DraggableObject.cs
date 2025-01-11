using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Handles the logic for a draggable object that can be dragged by the player.
/// </summary>
public class DraggableObject : MonoBehaviour
{
    [SerializeField] private ColliderChecker colliderChecker; // Reference to the collider checker for detecting colliders below
    [SerializeField] private Collider2D objectCollider; // The collider for the draggable object

    private Shelf _currentShelf; // Current shelf where the object is placed
    private bool _isDragged; // Flag to check if the object is currently being dragged
    private Tween _currentTween; // Tween for animation (e.g., falling animation)

    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; } // Reference to the object's sprite renderer

    private PlayerControls _playerControls; // Reference to player input controls
    private Camera _mainCamera; // Reference to the main camera

    private Vector3 _targetPosition; // Target position for the object while dragging
    [SerializeField] private float followSpeed = 10; // Speed at which the object follows the cursor during drag

    /// <summary>
    /// Initializes required references and input controls.
    /// </summary>
    private void Awake()
    {
        if (_mainCamera == null) _mainCamera = Camera.main; // Initialize the camera if not already assigned
        if (SpriteRenderer == null) SpriteRenderer = GetComponent<SpriteRenderer>(); // Initialize the SpriteRenderer if not already assigned
        if (colliderChecker == null) colliderChecker = GetComponent<ColliderChecker>(); // Initialize the ColliderChecker if not already assigned
        if (objectCollider == null) objectCollider = GetComponent<Collider2D>(); // Initialize the object collider if not already assigned
        _playerControls ??= new PlayerControls(); // Initialize the player controls
    }

    /// <summary>
    /// Enables the player input controls when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        _playerControls.Enable(); // Enable the controls
    }

    /// <summary>
    /// Disables the player input controls when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        _playerControls.Disable(); // Disable the controls
    }

    /// <summary>
    /// Updates the object's position while being dragged based on the player's pointer position.
    /// </summary>
    private void Update()
    {
        if (!_isDragged) return; // If not being dragged, exit early

        // Get the current world position of the pointer
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>());
        worldPosition.z = 0; // Set the Z position to 0 to keep it 2D

        // Set the target position for the object
        _targetPosition = worldPosition;

        // Move the object smoothly towards the target position
        transform.position = Vector3.Lerp(transform.position, _targetPosition, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Starts the dragging process by scaling the object and updating its sorting order.
    /// </summary>
    public void StartDragging()
    {
        _isDragged = true; // Set dragging state to true

        // Scale the object to indicate that it's being dragged
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f);

        // Update the sprite's sorting order to bring it to the front
        SpriteRenderer.sortingOrder = 50;

        // Kill any existing tween to reset any ongoing animation
        if (_currentTween != null && !_currentTween.IsComplete()) _currentTween.Kill(true);

        // Remove the object from its current shelf (if any)
        if (_currentShelf != null)
            _currentShelf.RemoveObjectFromShelf(this);
    }

    /// <summary>
    /// Stops the dragging process and checks where to drop the object.
    /// </summary>
    public void StopDragging()
    {
        _isDragged = false; // Set dragging state to false

        // Reset the object's scale back to normal
        transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f);

        // Check for colliders below the object to determine where it can be placed
        Collider2D upperCollider2D = colliderChecker.GetCollidersBelow()
            .OrderByDescending(obj => obj.bounds.max.y) // Sort by Y position to find the closest one
            .FirstOrDefault(colliderBelow => !(colliderBelow.bounds.size.x < objectCollider.bounds.size.x)); // Make sure the object can fit on the shelf

        // If a suitable collider is found (e.g., a shelf), place the object on it
        if (upperCollider2D != null)
        {
            _currentShelf = upperCollider2D.gameObject.GetComponent<Shelf>();
            _currentShelf.AddObjectToShelf(this); // Add the object to the shelf

            // Determine the final position of the object
            Vector3 finalPosition = transform.position.y >= upperCollider2D.bounds.max.y
                ? new Vector3(transform.position.x, upperCollider2D.bounds.max.y, 0)
                : new Vector3(transform.position.x, transform.position.y, 0);

            // Calculate the fall time based on gravity
            float fallTime = Mathf.Sqrt(2 * (transform.position.y - finalPosition.y) / 9.8f);

            // Animate the object falling to the shelf's position
            _currentTween = transform.DOMove(finalPosition, fallTime)
                .SetEase(Ease.OutBounce); // Use a bounce effect when it lands
        }
    }
}
