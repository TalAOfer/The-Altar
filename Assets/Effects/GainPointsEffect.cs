using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsEffect : Effect
{
    public int amount;
     
    public void Initialize(int amount)
    {
        this.amount = amount;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        yield return StartCoroutine(context.InitiatingCard.GainPoints(amount, false));

        yield return new WaitForSeconds(postdelay);
    }
}
