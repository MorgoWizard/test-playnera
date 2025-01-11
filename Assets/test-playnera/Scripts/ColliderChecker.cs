using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks for colliders below the object within a certain range, based on the object's bounds and camera view.
/// </summary>
public class ColliderChecker : MonoBehaviour
{
    [SerializeField] private ContactFilter2D contactFilter; // Filter for collider detection

    private Camera _mainCamera; // Reference to the main camera
    private Collider2D _collider; // The collider component of the object

    /// <summary>
    /// Initializes references to the main camera and collider on the object.
    /// </summary>
    private void Awake()
    {
        _mainCamera = Camera.main;
        _collider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Gets a list of colliders below the object within a box defined by the object's bounds and the camera's viewport.
    /// </summary>
    /// <returns>A list of colliders below the object, or null if the collider is missing.</returns>
    public List<Collider2D> GetCollidersBelow()
    {
        if (_collider == null)
        {
            Debug.LogError("Отсутствует Collider2D на объекте.");
            return null;
        }

        // Get the bounds of the object and calculate the bottom edge
        Bounds objectBounds = _collider.bounds;
        float bottomY = objectBounds.min.y;

        // Get the Y position of the camera's bottom edge in world coordinates
        float cameraBottomY = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        // Create a box size based on the object width and distance from the camera's bottom to the object's bottom
        Vector2 boxSize = new Vector2(objectBounds.size.x, bottomY - cameraBottomY);

        // Create a box center at the midpoint between the object bottom and the camera bottom
        Vector2 boxCenter = new Vector2(objectBounds.center.x, (bottomY + cameraBottomY) / 2);

        // Create a list to store the colliders detected in the box
        List<Collider2D> results = new();
        Physics2D.OverlapBox(boxCenter, boxSize, 0, contactFilter, results);

        return results;
    }

    /// <summary>
    /// Draws the detection area in the Scene view for visualization.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_collider == null || _mainCamera == null) return;

        // Get the bounds of the object and calculate the bottom edge
        Bounds objectBounds = _collider.bounds;
        float bottomY = objectBounds.min.y;
        float cameraBottomY = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        // Create the box size and center for the Gizmos visualization
        Vector2 boxSize = new Vector2(objectBounds.size.x, bottomY - cameraBottomY);
        Vector2 boxCenter = new Vector2(objectBounds.center.x, (bottomY + cameraBottomY) / 2);

        // Draw the wireframe of the detection box in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
