using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseRoomState<TContext> : IRoomState where TContext : RoomStateMachine
{
    protected TContext _ctx;
    protected FloorManager FloorCtx => _ctx.FloorCtx;

    public BaseRoomState(TContext ctx)
    {
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
}

