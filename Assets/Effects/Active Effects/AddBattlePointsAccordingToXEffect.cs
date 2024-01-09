using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBattlePointsAccordingToXEffect : ActiveEffect
{
    public override void SendEvent()
    {
        events.GetXAmount.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        int xAmount = (int)response;
        parentCard.attackPointsModifiers.Add(new BattlePointModifier(ModifierType.Addition, xAmount));
        //SendLog(xAmount);
        yield return null;
    }

    private void SendLog(int amount)
    {
        string log = parentCard + "got +" + amount + "attack points for Xs on map";
        events.AddLogEntry.Raise(this, log);
    }
}
