using System.Collections;

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
}

public interface IRoomState
{
    // Method to be called when entering the state
    IEnumerator EnterState();

    // Method to be called when exiting the state
    IEnumerator ExitState();
}

