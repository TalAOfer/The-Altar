using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    public CardInteractionBase cardInteraction;

    public virtual void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        cardInteraction.gameObject.SetActive(false);
    }

    public virtual void OnRoomFinishedLerping()
    {
        cardInteraction.gameObject.SetActive(true);
    }
    public virtual void OnRoomFinished() 
    {
        cardInteraction.gameObject.SetActive(false);
    }

}

public enum RoomType
{
    Battle,
    CardPicking
}