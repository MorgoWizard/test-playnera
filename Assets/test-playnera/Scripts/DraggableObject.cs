using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    [SerializeField] private ColliderChecker colliderChecker;
    [SerializeField] private Collider2D objectCollider;
    
    private Shelf _currentShelf;
    private bool _isDragged;
    private Tween _currentTween;
    [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
    
    private PlayerControls _playerControls;
    private Camera _mainCamera;
    
    Vector3 _targetPosition;
    
    [SerializeField] private float followSpeed = 10;

    private void Awake()
    {
        if(_mainCamera ==null) _mainCamera = Camera.main;
        if(SpriteRenderer == null) SpriteRenderer = GetComponent<SpriteRenderer>();
        if(colliderChecker == null) colliderChecker = GetComponent<ColliderChecker>();
        if(objectCollider == null) objectCollider = GetComponent<Collider2D>();
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void Update()
    {
        if (!_isDragged) return;
        
        Vector3 worldPosition =
            _mainCamera.ScreenToWorldPoint(_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>());
        worldPosition.z = 0;
        
        _targetPosition = worldPosition;
        
        transform.position = Vector3.Lerp(transform.position, _targetPosition, followSpeed * Time.deltaTime);
    }

    public void StartDragging()
    {
        _isDragged = true;

        SpriteRenderer.sortingOrder = 50;
        
        if(_currentTween != null && !_currentTween.IsComplete()) _currentTween.Kill(true);
        
        if (_currentShelf != null)
            _currentShelf.RemoveObjectFromShelf(this);
    }
    
    public void StopDragging()
    {
        _isDragged = false;

        Collider2D upperCollider2D = colliderChecker.GetCollidersBelow().OrderByDescending(obj => obj.bounds.max.y).FirstOrDefault(colliderBelow => !(colliderBelow.bounds.size.x < objectCollider.bounds.size.x));
        
        if (upperCollider2D != null)
        {
            _currentShelf = upperCollider2D.gameObject.GetComponent<Shelf>();
            _currentShelf.AddObjectToShelf(this);
            Vector3 finalPosition = new Vector3(transform.position.x, upperCollider2D.bounds.max.y, 0);
            float fallTime = Mathf.Sqrt(2 * (transform.position.y - finalPosition.y) / 9.8f);
            _currentTween = transform.DOMove(finalPosition, fallTime)
                .SetEase(Ease.OutBounce);
        }
    }
}
