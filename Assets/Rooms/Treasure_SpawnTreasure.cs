using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Treasure_SpawnTreasure : BaseRoomState
{
    public Treasure_SpawnTreasure(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        GameObject treasureChestGO = _sm.InstantiatePrefab(Prefabs.TreasureChest, Prefabs.TreasureChest.transform.position, Quaternion.identity, _sm.transform);
        TreasureChest treasureChest = treasureChestGO.GetComponent<TreasureChest>();
        _ctx.currentTreasureChest = treasureChest;
        treasureChest.Initialize(_sm.Room.Reward);
        yield return treasureChest.Fall();
        SwitchTo(States.OpenTreasure());
    }


}
