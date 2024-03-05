using System.Collections;
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

    public virtual void HandlePointerEnter(Card card, PointerEventData eventData) { }

    public virtual void HandlePointerExit(Card card, PointerEventData eventData) { }

    public virtual void HandleBeginDrag(Card card, PointerEventData eventData) { }

    public virtual void HandleEndDrag(Card card, PointerEventData eventData) { }

    public virtual void HandleDrag() { }

    public virtual void HandlePointerClick(Card card, PointerEventData eventData) { }
}

public interface IRoomState
{
    // Method to be called when entering the state
    IEnumerator EnterState();

    // Method to be called when exiting the state
    IEnumerator ExitState();
    void HandlePointerEnter(Card card, PointerEventData eventData);
    void HandlePointerExit(Card card, PointerEventData eventData);

    void HandleBeginDrag(Card card, PointerEventData eventData);
    void HandleDrag();
    void HandleEndDrag(Card card, PointerEventData eventData);
    void HandlePointerClick(Card card, PointerEventData eventData);
}

