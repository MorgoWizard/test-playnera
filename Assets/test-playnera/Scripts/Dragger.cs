using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dragger : MonoBehaviour
{
    //[SerializeField] private Collider2D dragCollider;
    [SerializeField] private ContactFilter2D contactFilter;

    private Camera _mainCamera;
    
    private PlayerControls _playerControls;
    
    private DraggableObject _draggableObject;

    private void Awake()
    {
        _playerControls ??= new PlayerControls();
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        _playerControls.Gameplay.PointerPress.started += OnDragStarted;
        _playerControls.Gameplay.PointerPress.canceled += OnDragCanceled;
    }
    
    private void OnDragCanceled(InputAction.CallbackContext context)
    {
        if (_draggableObject)
        {
            _draggableObject.SetIsDragging(false);
            _draggableObject = null;
        }
    }

    private void OnDragStarted(InputAction.CallbackContext context)
    {
        Vector3 worldPosition =
            _mainCamera.ScreenToWorldPoint(_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>());
        worldPosition.z = 0;
        
        List<Collider2D> overlaps = new();
        Physics2D.OverlapPoint(worldPosition, contactFilter, overlaps);
        
        _draggableObject = overlaps.FirstOrDefault()?.GetComponent<DraggableObject>();
        
        if(_draggableObject == null) return;
        
        _draggableObject.SetIsDragging(true);
    }

    private void OnDisable()
    {
        _playerControls.Disable();
        _playerControls.Gameplay.PointerPress.started -= OnDragStarted;
        _playerControls.Gameplay.PointerPress.canceled -= OnDragCanceled;
    }
}
