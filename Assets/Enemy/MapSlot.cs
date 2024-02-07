using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSlot : MonoBehaviour
{
    public MapSlotState slotState;
    public int index;
    [SerializeField] private Collider2D coll;
    [SerializeField] private SpriteRenderer sr;

    public IEnumerator SetSlotState(MapSlotState newState)
    {
        slotState = newState;
        switch (slotState)
        {
            case MapSlotState.Free:
                break;
            case MapSlotState.Enemy:
                break;
            case MapSlotState.Grave:
                break;
        }

        yield return null;
    }

    public void SetSortingLayer(string sortingLayerName)
    {
        sr.sortingLayerName = sortingLayerName;
    }

    public void SetSortingOrder(int index)
    {
        sr.sortingOrder = index;
    }

    public string GetSortingLayerName()
    {
        return sr.sortingLayerName;
    }
}

public enum MapSlotState
{
    Free,
    Enemy,
    Grave
}
