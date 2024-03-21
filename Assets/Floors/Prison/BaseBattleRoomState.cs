using UnityEngine.EventSystems;

public abstract class BaseBattleRoomState : BaseRoomState<BattleRoomStateMachine>
{
    public BaseBattleRoomState(BattleRoomStateMachine ctx) : base(ctx)
    {
    }
}