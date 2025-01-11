using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the camera movement based on player input and drag interactions.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Background Settings")]
    [Tooltip("The SpriteRenderer of the background. Used to determine camera movement limits.")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Camera Movement Settings")]
    [Tooltip("Speed at which the camera moves when dragging.")]
    [SerializeField] private float dragSpeed = 1.0f;

    [Tooltip("Threshold as a percentage of the screen width where dragging affects camera movement.")]
    [SerializeField, Range(0, 0.5f)] private float draggingMoveThreshold = 0.1f;

    [Header("Dependencies")]
    [Tooltip("Reference to the dragger script controlling object dragging.")]
    [SerializeField] private Dragger dragger;

    private Vector2 _backgroundBounds; // Horizontal boundaries of the background
    private Vector2 _draggingMoveBounds; // Screen bounds for detecting dragging zones
    private float _cameraHalfWidth; // Half the width of the camera view
    private PlayerControls _playerControls; // Input system actions
    private bool _isPointerPressed; // Tracks if the pointer is currently pressed
    
    /// <summary>
    /// Initializes player controls and calculates initial bounds.
    /// </summary>
    private void Awake()
    {
        _playerControls ??= new PlayerControls();

        // Calculate background bounds
        _backgroundBounds = new Vector2(
            transform.position.x - backgroundRenderer.bounds.max.x,
            transform.position.x + backgroundRenderer.bounds.max.x
        );

        // Calculate dragging bounds as a fraction of the screen width
        float screenWidth = Screen.width;
        _draggingMoveBounds = new Vector2(
            screenWidth * draggingMoveThreshold,
            screenWidth - screenWidth * draggingMoveThreshold
        );

        // Calculate the camera's half-width based on orthographic size and aspect ratio
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            _cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
    }
    
    /// <summary>
    /// Subscribes to input actions when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Gameplay.PointerPress.started += OnPointerPressStarted;
        _playerControls.Gameplay.PointerPress.canceled += OnPointerPressCancelled;
    }

    /// <summary>
    /// Unsubscribes from input actions when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        _playerControls.Disable();
        _playerControls.Gameplay.PointerPress.started -= OnPointerPressStarted;
        _playerControls.Gameplay.PointerPress.canceled -= OnPointerPressCancelled;
    }

    /// <summary>
    /// Updates the camera position based on drag or pointer movement.
    /// </summary>
    private void Update()
    {
        if (dragger.IsDragging)
        {
            // Get the current pointer position
            Vector2 pointerPosition = _playerControls.Gameplay.PointerPosition.ReadValue<Vector2>();

            // Check if the pointer is in the left or right drag zones
            bool pointerInLeftDragSide = pointerPosition.x <= _draggingMoveBounds.x;
            bool pointerInRightDragSide = pointerPosition.x >= _draggingMoveBounds.y;

            // Move the camera if in drag zones
            if (pointerInLeftDragSide || pointerInRightDragSide)
            {
                if (pointerInLeftDragSide)
                {
                    MoveCamera(-dragSpeed * Time.fixedDeltaTime);
                }
                if (pointerInRightDragSide)
                {
                    MoveCamera(dragSpeed * Time.fixedDeltaTime);
                }
            }
        }
        else if (_isPointerPressed)
        {
            // Move camera based on pointer delta when dragging is inactive
            Vector2 pointerDelta = _playerControls.Gameplay.PointerDelta.ReadValue<Vector2>();
            MoveCamera(-pointerDelta.x * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Moves the camera horizontally while keeping it within background boundaries.
    /// </summary>
    /// <param name="moveAmount">Amount to move the camera horizontally.</param>
    private void MoveCamera(float moveAmount)
    {
        float newX = Mathf.Clamp(
            transform.position.x + moveAmount,
            _backgroundBounds.x + _cameraHalfWidth,
            _backgroundBounds.y - _cameraHalfWidth
        );

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    /// <summary>
    /// Handles the start of pointer press input.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnPointerPressStarted(InputAction.CallbackContext context)
    {
        _isPointerPressed = true;
    }

    /// <summary>
    /// Handles the end of pointer press input.
    /// </summary>
    /// <param name="context">The input action callback context.</param>
    private void OnPointerPressCancelled(InputAction.CallbackContext context)
    {
        _isPointerPressed = false;
    }
}
