using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private float dragSpeed = 1.0f;
    [SerializeField, Range(0, 0.5f)] private float draggingMoveThreshold = 0.1f;
    
    [SerializeField] private Dragger dragger;

    private Vector2 _backgroundBounds;
    private Vector2 _draggingMoveBounds;

    private float _cameraHalfWidth;
    
    private PlayerControls _playerControls;

    private bool _isPointerPressed;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _backgroundBounds = new Vector2(transform.position.x-backgroundRenderer.bounds.max.x, transform.position.x+backgroundRenderer.bounds.max.x);
        float screenWidth = Screen.width;
        _draggingMoveBounds = new Vector2(screenWidth * draggingMoveThreshold, screenWidth - screenWidth * draggingMoveThreshold);
        Camera mainCamera = Camera.main;
        if (mainCamera != null) _cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
    }

    private void Update()
    {
        if (dragger.IsDragging)
        {
            Vector2 pointerPosition = _playerControls.Gameplay.PointerPosition.ReadValue<Vector2>();
            bool pointerInLeftDragSide = pointerPosition.x <= _draggingMoveBounds.x;
            bool pointerInRightDragSide = pointerPosition.x >= _draggingMoveBounds.y;
            
            if (pointerInLeftDragSide || pointerInRightDragSide)
            {
                if (pointerInLeftDragSide)
                {
                    MoveCamera(-dragSpeed*Time.fixedDeltaTime);
                }
                if (pointerInRightDragSide)
                {
                    MoveCamera(dragSpeed*Time.fixedDeltaTime);
                }
            }
        }
        else
        {
            if (_isPointerPressed)
            {
                Vector2 pointerDelta = _playerControls.Gameplay.PointerDelta.ReadValue<Vector2>();
                MoveCamera(-pointerDelta.x*Time.fixedDeltaTime);
            }
        }
    }

    private void MoveCamera(float moveAmount)
    {
        float newX = Mathf.Clamp(transform.position.x + moveAmount, _backgroundBounds.x + _cameraHalfWidth, _backgroundBounds.y - _cameraHalfWidth);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void OnPointerPressStarted(InputAction.CallbackContext context)
    {
        _isPointerPressed = true;
    }
    
    private void OnPointerPressCancelled(InputAction.CallbackContext context)
    {
        _isPointerPressed = false;
    }


    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Gameplay.PointerPress.started += OnPointerPressStarted;
        _playerControls.Gameplay.PointerPress.canceled += OnPointerPressCancelled;
    }

    private void OnDisable()
    {
        _playerControls.Disable();
        _playerControls.Gameplay.PointerPress.started -= OnPointerPressStarted;
        _playerControls.Gameplay.PointerPress.canceled -= OnPointerPressCancelled;
    }
}
