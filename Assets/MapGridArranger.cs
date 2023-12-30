using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapGridArranger : MonoBehaviour
{
    [SerializeField] private GameObject mapSlotPrefab;
    public List<MapSlot> MapSlots; // List of 9 transforms
    [SerializeField] private List<float> rowY;
    [SerializeField] private List<float> colX;

    public void OnMapCardDead(Component sender, object data)
    {
        int slotIndex = (int)data;
        MapSlot slot = MapSlots[slotIndex];
        slot.SetCardState(MapSlotState.Done);
    }

#if UNITY_EDITOR
    [Button]
    public void SpawnAndArrange()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject slot = PrefabUtility.InstantiatePrefab(mapSlotPrefab) as GameObject;
            slot.transform.SetParent(transform, false);
            transform.localScale = Vector3.one;
            MapSlots.Add(slot.GetComponent<MapSlot>());
        }

        ArrangeGrid();
    }
#endif

    [Button]

    public void ArrangeGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            MapSlot slot = MapSlots[i];
            if (slot == null) continue;

            int row = i / 3;
            int col = i % 3;

            Vector3 localPosition = new Vector3(
                colX[col],
                rowY[row],
                0
            );

            slot.index = i;
            slot.transform.localPosition = localPosition;
            string rowName = "";
            string colName = "";

            switch (row)
            {
                case 0:
                    rowName = "Top";
                    break;
                case 1:
                    rowName = "Center";
                    break;
                case 2:
                    rowName = "Bottom";
                    break;
            }

            switch (col)
            {
                case 0:
                    colName = "Left";
                    break;
                case 1:
                    colName = "Middle";
                    break;
                case 2:
                    colName = "Right";
                    break;
            }

            slot.name = rowName + " " + colName;
        }
    }
}