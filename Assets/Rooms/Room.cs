using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    public CardInteractionBase cardInteraction;
    public CustomAnimator animator;

    public virtual void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        cardInteraction.gameObject.SetActive(false);
    }

    public abstract IEnumerator AnimateDown();
    protected IEnumerator WaitForAnimationEnd(string animationName)
    {
        float animationDuration = animator.GetAnimationDuration(animationName);
        yield return new WaitForSeconds(animationDuration);
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