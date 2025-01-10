using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    [SerializeField] private ContactFilter2D contactFilter;

    private Camera _mainCamera;
    private Collider2D _collider;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _collider = GetComponent<Collider2D>();
    }

    public List<Collider2D> GetCollidersBelow()
    {
        if (_collider == null)
        {
            Debug.LogError("Отсутствует Collider2D на объекте.");
            return null;
        }
        
        Bounds objectBounds = _collider.bounds;
        
        float bottomY = objectBounds.min.y;
        
        float cameraBottomY = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        
        Vector2 boxSize = new Vector2(objectBounds.size.x, bottomY - cameraBottomY);
        
        Vector2 boxCenter = new Vector2(objectBounds.center.x, (bottomY + cameraBottomY) / 2);
        
        List<Collider2D> results = new();
        Physics2D.OverlapBox(boxCenter, boxSize, 0, contactFilter, results);

        return results;
    }

    private void OnDrawGizmos()
    {
        if (_collider == null || _mainCamera == null) return;

        Bounds objectBounds = _collider.bounds;
        float bottomY = objectBounds.min.y;
        float cameraBottomY = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        Vector2 boxSize = new Vector2(objectBounds.size.x, bottomY - cameraBottomY);
        Vector2 boxCenter = new Vector2(objectBounds.center.x, (bottomY + cameraBottomY) / 2);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
