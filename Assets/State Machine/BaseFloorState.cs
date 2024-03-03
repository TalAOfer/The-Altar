using System;
using System.Collections;

public abstract class BaseFloorState
{
    public NewFloorManager _ctx;
    public BaseFloorState(NewFloorManager stateMachine)
    {
        _ctx = stateMachine;
    }
    public virtual IEnumerator EnterState() { yield break; }
    public virtual IEnumerator ExitState() { yield break; }
}
