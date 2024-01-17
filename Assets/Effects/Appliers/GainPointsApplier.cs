using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsApplier : EffectApplier
{
    public int pointsToGain;

    public void Initialize(int pointsToGain)
    {
        this.pointsToGain = pointsToGain;
    }
    public override IEnumerator ApplyEffect(Card target)
    {
        yield return target.GainPoints(pointsToGain);
    }
}
    
