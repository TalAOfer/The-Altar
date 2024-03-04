using System;
using System.Collections;

public abstract class BaseFloorState
{
    public FloorManager _ctx;
    public BaseFloorState(FloorManager stateMachine)
    {
        _ctx = stateMachine;
    }
    public virtual IEnumerator EnterState() { yield break; }
    public virtual IEnumerator ExitState() { yield break; }
}
