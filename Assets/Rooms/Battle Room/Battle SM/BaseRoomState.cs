using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseRoomState : IRoomState
{
    protected RoomStateMachine _sm;
    protected SMContext _ctx;
    protected StateFactory States => _sm.States; 
    protected FloorManager FloorCtx => _sm.FloorCtx;
    protected RunData RunData => FloorCtx.RunData;
    protected EventRegistry Events => _sm.Events;
    protected PrefabRegistry Prefabs => _sm.Prefabs;

    protected void SwitchTo(IRoomState newState)
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
}

public interface IRoomState
{
    // Method to be called when entering the state
    IEnumerator EnterState();

    // Method to be called when exiting the state
    IEnumerator ExitState();
    void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData);
    void HandlePlayerCardPointerExit(Card card, PointerEventData eventData);
    void HandlePlayerCardPointerClick(Card card, PointerEventData eventData);

    void HandlePlayerCardBeginDrag(Card card, PointerEventData eventData);
    void HandlePlayerCardDrag();
    void HandlePlayerCardEndDrag(Card card, PointerEventData eventData);
    void HandleEnemyCardPointerEnter(Card card, PointerEventData eventData);
    void HandleEnemyCardPointerExit(Card card, PointerEventData eventData);
    void HandleEnemyCardPointerClick(Card card, PointerEventData eventData);

    void HandleEnemyCardBeginDrag(Card card, PointerEventData eventData);
    void HandleEnemyCardDrag();
    void HandleEnemyCardEndDrag(Card card, PointerEventData eventData);

    void OnHandColliderPointerEnter(HandCollisionDetector HandCollisionManager, PointerEventData data);
    void OnHandColliderPointerExit(HandCollisionDetector HandCollisionManager, PointerEventData data);
    void OnAbilityClicked(AbilityManager abilityManager, Ability ability);

}

