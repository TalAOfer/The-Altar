using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseRoomState
{
    protected RoomStateMachine _sm;
    protected SMContext _ctx;
    protected StateFactory States => _sm.States; 
    protected RunData RunData => _sm.RunData;
    protected EventRegistry Events => _sm.Events;
    protected PrefabRegistry Prefabs => _sm.Prefabs;

    protected void SwitchTo(BaseRoomState newState)
    {
        _sm.SwitchState(newState);
    }

    public BaseRoomState(RoomStateMachine sm, SMContext ctx)
    {
        _sm = sm;
        _ctx = ctx;
    }

    public virtual IEnumerator EnterState()
    {
        yield break;
    }

    public virtual IEnumerator ExitState()
    {
        yield break;
    }

    public virtual void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData) { }

    public virtual void HandlePlayerCardPointerExit(Card card, PointerEventData eventData) { }

    public virtual void HandlePlayerCardBeginDrag(Card card, PointerEventData eventData) { }

    public virtual void HandlePlayerCardEndDrag(Card card, PointerEventData eventData) { }

    public virtual void HandlePlayerCardDrag() { }

    public virtual void HandlePlayerCardPointerClick(Card card, PointerEventData eventData) { }

    public virtual void HandleEnemyCardPointerEnter(Card card, PointerEventData eventData) { }

    public virtual void HandleEnemyCardPointerExit(Card card, PointerEventData eventData) { }

    public virtual void HandleEnemyCardBeginDrag(Card card, PointerEventData eventData) { }

    public virtual void HandleEnemyCardEndDrag(Card card, PointerEventData eventData) { }

    public virtual void HandleEnemyCardDrag() { }

    public virtual void HandleEnemyCardPointerClick(Card card, PointerEventData eventData) { }

    public virtual void OnHandColliderPointerEnter(HandCollisionDetector HandCollisionManager, PointerEventData data) { }

    public virtual void OnHandColliderPointerExit(HandCollisionDetector HandCollisionManager, PointerEventData data) { }

    public virtual void OnAbilityClicked(AbilityManager abilityManager, Ability ability) {}

    public virtual void OnRoomButtonClicked(CustomButton button, int index) {}

    public virtual void OnDoorClicked(RoomBlueprint nextRoom) {}
}


