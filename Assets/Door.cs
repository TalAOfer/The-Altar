using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Door : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AllEvents events;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D coll;
    public void OpenDoor()
    {
        sr.sprite = openDoor;
        coll.enabled = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("done");
    }
}
