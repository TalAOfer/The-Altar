using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        yield return target.CalcHurtPoints(amount);
        target.TakeDamage(this);
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        throw new System.NotImplementedException();
    }
}
