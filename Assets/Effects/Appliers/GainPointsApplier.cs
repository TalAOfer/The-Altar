using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target)
    {
        yield return target.GainPoints(GetAmount());
    }
}
    
