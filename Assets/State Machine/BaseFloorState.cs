using System.Collections;

public abstract class BaseFloorState
{
    public FloorStateMachine _ctx;
    public BaseFloorState(FloorStateMachine stateMachine)
    {
        _ctx = stateMachine;
    }
    public virtual IEnumerator EnterState() { yield break; }
    public virtual IEnumerator ExitState() { yield break; }
}
