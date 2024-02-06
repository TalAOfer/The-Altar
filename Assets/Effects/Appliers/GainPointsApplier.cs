using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        data.events.OnEffectApplied.Raise(this, new EffectIndication("Gained +" + amount.ToString() + " points", target));
        yield return target.GainPoints(amount);
    }
}
    
