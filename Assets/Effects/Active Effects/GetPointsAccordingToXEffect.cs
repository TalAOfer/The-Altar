using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPointsAccordingToXEffect : ActiveEffect
{
    public override void SendEvent()
    {
        events.GetXAmount.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        int xAmount = (int)response;
        parentCard.attackPointsModifiers.Add(new BattlePointModifier(ModifierType.Addition, xAmount));
        yield return null;
    }
}
