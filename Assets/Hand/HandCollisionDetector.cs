using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCollisionDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AllEvents events;
    [SerializeField] private HandManager hand;
    public BoxCollider2D coll;

    public void OnPointerEnter(PointerEventData eventData)
    {
        events.OnCursorEnterHand.Raise(this, eventData);
        //Debug.Log("Pointer enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        events.OnCursorExitHand.Raise(this, eventData);
        //Debug.Log("Pointer exit");
    }
}
