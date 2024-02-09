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

    public virtual void OnRoomFinishedLerping()
    {
        cardInteraction.gameObject.SetActive(true);
    }
    public virtual void OnRoomFinished() 
    {
        cardInteraction.gameObject.SetActive(false);
    }
    public IEnumerator AnimateDown()
    {
        animator.PlayAnimation("Down");
        yield return WaitForAnimationEnd("Down");
    }
    protected IEnumerator WaitForAnimationEnd(string animationName)
    {
        float animationDuration = animator.GetAnimationDuration(animationName);
        yield return new WaitForSeconds(animationDuration);
    }

}

public enum RoomType
{
    Battle,
    CardPicking,
    PlaytestCardGain,
    PlaytestWin,
}