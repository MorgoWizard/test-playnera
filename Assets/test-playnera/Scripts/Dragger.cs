using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the logic for dragging objects in the game world.
/// </summary>
public class Dragger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Contact filter used to detect draggable objects.")]
    [SerializeField] private ContactFilter2D contactFilter;

    private Camera _mainCamera; // Main camera reference for converting screen to world points
    private PlayerControls _playerControls; // Input system controls
    private DraggableObject _draggableObject; // Currently dragged object

    /// <summary>
    /// Indicates whether an object is currently being dragged.
    /// </summary>
    public bool IsDragging { get; private set; }

    /// <summary>
    /// Initializes the input system and camera reference.
    /// </summary>
    private void Awake()
    {
        _playerControls ??= new PlayerControls();
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Subscribes to input events when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Gameplay.PointerPress.started += OnDragStarted;
        _playerControls.Gameplay.PointerPress.canceled += OnDragCanceled;
    }

    /// <summary>
    /// Unsubscribes from input events when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        _playerControls.Disable();
        _playerControls.Gameplay.PointerPress.started -= OnDragStarted;
        _playerControls.Gameplay.PointerPress.canceled -= OnDragCanceled;
    }
    
    /// <summary>
    /// Handles the logic for starting a drag operation.
    /// </summary>
    /// <param name="context">The input action context.</param>
    private void OnDragStarted(InputAction.CallbackContext context)
    {
        // Convert pointer position from screen to world coordinates
        Vector3 worldPosition = 
            _mainCamera.ScreenToWorldPoint(_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>());
        worldPosition.z = 0;

        // Detect colliders at the pointer position
        List<Collider2D> overlaps = new();
        Physics2D.OverlapPoint(worldPosition, contactFilter, overlaps);

        // Get the first draggable object from the overlaps
        _draggableObject = overlaps.FirstOrDefault()?.GetComponent<DraggableObject>();

        if (_draggableObject == null) return;

        // Start dragging the object
        _draggableObject.StartDragging();
        IsDragging = true;
    }

    /// <summary>
    /// Handles the logic for canceling a drag operation.
    /// </summary>
    /// <param name="context">The input action context.</param>
    private void OnDragCanceled(InputAction.CallbackContext context)
    {
        if (_draggableObject == null) return;

        // Stop dragging the object
        _draggableObject.StopDragging();
        _draggableObject = null;
        IsDragging = false;
    }
}
