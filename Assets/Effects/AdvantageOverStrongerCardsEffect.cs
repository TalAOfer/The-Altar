using System.Collections;
using UnityEngine;

public class AdvantageOverStrongerCardsEffect : Effect
{
    public int amount;

    public void Initialize(int amount)
    {
        this.amount = amount;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        if (context.InitiatingCard.points < context.OtherCard.points)
        {
            yield return StartCoroutine(context.InitiatingCard.GainPoints(amount, false));
        }

        yield return new WaitForSeconds(postdelay);
    }
}
