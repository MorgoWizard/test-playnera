using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private bool _isDragging;
    
    private PlayerControls _playerControls;
    private Camera _mainCamera;
    
    Vector3 _targetPosition;
    
    [SerializeField] private float followSpeed = 10;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    public void SetIsDragging(bool state)
    {
        _isDragging = state;
    }

    private void Update()
    {
        if (!_isDragging) return;
        
        Vector3 worldPosition =
            _mainCamera.ScreenToWorldPoint(_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>());
        worldPosition.z = 0;
        
        _targetPosition = worldPosition;
        
        transform.position = Vector3.Lerp(transform.position, _targetPosition, followSpeed * Time.deltaTime);
    }
}
