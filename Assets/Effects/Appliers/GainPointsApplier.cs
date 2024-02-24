using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        RaiseEffectAppliedEvent(target, amount);
        yield return target.GainPoints(amount);
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Gained +" + amount.ToString() + " points";
    }
}
    
