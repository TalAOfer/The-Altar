using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MapSlotState slotState;
    public int index;
    [SerializeField] private Collider2D coll;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color doneColor;
    [SerializeField] private AllEvents events;

    private void Awake()
    {
        sr.color = defaultColor;
    }

    public void SetCardState(MapSlotState newState)
    {
        slotState = newState;
        switch (slotState)
        {
            case MapSlotState.Free:
                coll.enabled = true;
                break;
            case MapSlotState.Occupied:
                coll.enabled = false;
                break;
            case MapSlotState.Done:
                coll.enabled = false;
                sr.color = doneColor;
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        events.OnMapSlotClicked.Raise(this, index);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sr.color = defaultColor;
    }
}

public enum MapSlotState
{
    Free,
    Occupied,
    Done
}
