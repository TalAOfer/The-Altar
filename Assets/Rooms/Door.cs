using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AllEvents events;
    [SerializeField] private Color darkerWhite;
    [SerializeField] private Color white;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D coll;
    public FloorManager floorManager;

    public void Initialize(FloorManager floorManager)
    {
       this.floorManager = floorManager;
    }

    private void OnEnable()
    {
        sr.color = white;
    }
    public void OpenDoor()
    {
        sr.color = darkerWhite;
        sr.sprite = openDoor;
        coll.enabled = true;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        floorManager.NextRoom();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sr.color = darkerWhite;
    }
}
