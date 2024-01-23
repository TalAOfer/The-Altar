using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target)
    {
        yield return target.CalcHurtPoints(GetAmount());
        target.TakeDamage(this);
    }
}
