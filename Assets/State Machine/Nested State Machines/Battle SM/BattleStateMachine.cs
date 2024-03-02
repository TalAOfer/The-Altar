using System.Collections;
using System.Collections.Generic;

public abstract class BattleStateMachine : BaseFloorState
{
    private BaseBattleState _currentBattleState;
    public BattleBlueprint BattleBlueprint { get; private set; }
    public FloorStateMachine TopCtx { get; private set; }
    public List<Card> ActiveEnemies {  get; private set; }
    public EnemyCardSpawner EnemyCardSpawner {  get; private set; }

    protected BattleStateMachine(BattleBlueprint battleBlueprint, FloorStateMachine stateMachine) : base(stateMachine)
    {
        TopCtx = stateMachine;
        BattleBlueprint = battleBlueprint;
    }

    private void InitializeStateMachine(BaseBattleState state)
    {
        _currentBattleState = state;

        _ctx.StartCoroutine(_currentBattleState.EnterState());
    }

    public IEnumerator SwitchState(BaseBattleState newState)
    {
        yield return _ctx.StartCoroutine(_currentBattleState.ExitState());

        _currentBattleState = newState;

        _ctx.StartCoroutine(_currentBattleState.EnterState());
    }

    public override IEnumerator EnterState()
    {
        return base.EnterState();
    }
}
