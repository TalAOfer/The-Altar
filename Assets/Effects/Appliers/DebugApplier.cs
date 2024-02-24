using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        RaiseEffectAppliedEvent(target, amount);
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Happened!";
    }
}
