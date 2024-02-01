using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target)
    {
        data.events.OnEffectApplied.Raise(this, new EffectIndication("Gained +" + GetAmount().ToString() + " points", target));
        yield return target.GainPoints(GetAmount());
    }
}
    
