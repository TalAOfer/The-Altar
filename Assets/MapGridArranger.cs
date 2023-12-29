using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class MapGridArranger : MonoBehaviour
{
    public List<Transform> gridObjects; // List of 9 transforms
    public Vector3 centerPosition; // Center position of the grid
    public Vector3 topLeftPosition; // Top left position of the grid
    public Vector3 topRightPosition; // Top right position of the grid
    public Vector3 centerTopPosition; // Center top position of the grid

    private const int gridSize = 3; // Size of the grid (3x3)


    [Button]

    public void ArrangeGrid()
    {
        if (gridObjects.Count != gridSize * gridSize)
        {
            Debug.LogError("GridArranger requires exactly 9 transforms in the list.");
            return;
        }

        float horizontalSpacing = (topRightPosition - topLeftPosition).x / (gridSize - 1);
        // Assuming centerTopPosition is directly above topLeftPosition in the grid
        float verticalSpacing = Mathf.Abs(centerTopPosition.y - topLeftPosition.y);

        for (int i = 0; i < gridObjects.Count; i++)
        {
            if (gridObjects[i] == null) continue;

            int row = i / gridSize;
            int col = i % gridSize;

            Vector3 localPosition = new Vector3(
                topLeftPosition.x + col * horizontalSpacing,
                topLeftPosition.y - row * verticalSpacing, // Adjusting y-coordinate based on row
                topLeftPosition.z
            );

            gridObjects[i].localPosition = localPosition;
        }
    }
}