using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages draggable objects on a shelf and their rendering order.
/// </summary>
public class Shelf : MonoBehaviour
{
    private readonly List<DraggableObject> _objectsOnShelf = new();

    /// <summary>
    /// Adds an object to the shelf and updates its rendering order.
    /// </summary>
    /// <param name="objectToShelf">The object to add to the shelf.</param>
    public void AddObjectToShelf(DraggableObject objectToShelf)
    {
        _objectsOnShelf.Add(objectToShelf);
        UpdateRenderingOrder();
    }

    /// <summary>
    /// Removes an object from the shelf and updates the rendering order.
    /// </summary>
    /// <param name="objectToShelf">The object to remove from the shelf.</param>
    public void RemoveObjectFromShelf(DraggableObject objectToShelf)
    {
        _objectsOnShelf.Remove(objectToShelf);
        UpdateRenderingOrder();
    }

    /// <summary>
    /// Updates the rendering order of objects on the shelf based on their x-position.
    /// </summary>
    private void UpdateRenderingOrder()
    {
        var sortedObjects = _objectsOnShelf
            .OrderByDescending(obj => obj.transform.position.x)
            .ToList();

        for (int i = 0; i < sortedObjects.Count; i++)
        {
            sortedObjects[i].SpriteRenderer.sortingOrder = i;
        }
    }
}