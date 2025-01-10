using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    private readonly List<DraggableObject> _objectsOnShelf = new();

    public void AddObjectToShelf(DraggableObject objectToShelf)
    {
        _objectsOnShelf.Add(objectToShelf);
        SortObjectsRendering();
    }

    public void RemoveObjectFromShelf(DraggableObject objectToShelf)
    {
        _objectsOnShelf.Remove(objectToShelf);
        SortObjectsRendering();
    }

    private void SortObjectsRendering()
    {
        List<DraggableObject> objectsOnShelfSortedByX = _objectsOnShelf.OrderBy(obj => obj.transform.position.x).ToList();
        int counter = 0;
        foreach (var objectOnShelf in objectsOnShelfSortedByX)
        {
            objectOnShelf.SpriteRenderer.sortingOrder = counter;
            counter++;
        }
    }


}
