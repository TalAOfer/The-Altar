using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class ColorAdvantageEffect : Effect
{
    private CardColor ThisCardIsStrongerOn;
    private int advantageAmount;
    private int disadvantageAmount;

    public void Initialize(CardColor ThisCardIsStrongerOn, int advantageAmount, int disadvantageAmount)
    {
        this.ThisCardIsStrongerOn = ThisCardIsStrongerOn;
        this.advantageAmount = advantageAmount;
        this.disadvantageAmount=disadvantageAmount;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        Card attackingCard = context.InitiatingCard;
        Card attackedCard = context.OtherCard;

        yield return new WaitForSeconds(predelay);

        if (attackedCard.cardColor == ThisCardIsStrongerOn)
        {
            yield return StartCoroutine(attackingCard.GainPoints(advantageAmount, false));
        }

        else
        {
            attackingCard.hurtPoints = disadvantageAmount;
            attackingCard.TakeDamage();
        }

        yield return new WaitForSeconds(postdelay);
    }
}
