using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class TransformArranger : MonoBehaviour
{
    public List<Transform> transforms; // List of transforms to arrange
    public int columns = 5; // Number of columns, can be set dynamically
    public float xOffset = 1.0f; // Horizontal offset between items
    public float yOffset = 1.0f; // Vertical offset between items

    [Button("Arrange")]
    public void Arrange()
    {
        if (columns <= 0) return; // Guard against invalid column count

        // Calculate the starting X position so that the grid is centered around (0, 0)
        float gridWidth = (columns - 1) * xOffset;
        float startX = -gridWidth / 2;

        for (int i = 0; i < transforms.Count; i++)
        {
            int row = i / columns;
            int col = i % columns;

            // Calculate X position for the current item
            float xPosition = startX + (col * xOffset);

            // Calculate Y position for the current item. We find the total height if all items were in a single column
            // and use it to adjust the starting Y position to center the grid vertically around (0,0).
            int totalRows = (transforms.Count + columns - 1) / columns; // Ceiling of division to handle incomplete rows
            float gridHeight = (totalRows - 1) * yOffset;
            float startY = gridHeight / 2;

            float yPosition = startY - (row * yOffset);

            // Set the position of the current transform
            transforms[i].localPosition = new Vector3(xPosition, yPosition, transforms[i].localPosition.z);
        }
    }

    [Button("Populate")]
    public void PopulateTransforms()
    {
        transforms.Clear(); // Clear existing transforms list
        foreach (Transform child in transform) // Loop through each child transform
        {
            transforms.Add(child); // Add child transform to the list
        }
    }
}