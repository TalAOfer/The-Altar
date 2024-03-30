using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Treasure_OpenTreasure : BaseRoomState
{
    Treasure Treasure => _ctx.currentTreasureChest.Treasure;
    public Treasure_OpenTreasure(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        if (!Treasure.Items.Any())
        {
            SwitchTo(States.OpenDoors());
            yield break;
        }

        else
        {
            switch (Treasure.Items[0].ItemType)
            {
                case TreasureItemType.Money:
                    SpawnMoney();
                    SwitchTo(States.OpenTreasure());
                    Treasure.Items.RemoveAt(0);
                    break;
                case TreasureItemType.Pack:
                    SwitchTo(States.HandlePack());
                    break;
            }

            yield break;
        }
    }

    private void SpawnMoney()
    {

    }
}
