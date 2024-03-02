using System.Collections;

public abstract class BaseBattleState
{
    protected BattleStateMachine _ctx;
    protected BaseBattleState(BattleStateMachine ctx) 
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
