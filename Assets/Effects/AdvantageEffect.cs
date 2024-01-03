using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvantageEffect : Effect
{
    private WhoToChange whoToChange;
    private int advantageAmount;
    private int disadvantageAmount;
    private Decision decision;
    public void Initialize(WhoToChange whoToChange, Decision decision, int advantageAmount, int disadvantageAmount)
    {
        this.whoToChange = whoToChange;
        this.decision = decision;
        this.advantageAmount = advantageAmount;
        this.disadvantageAmount = disadvantageAmount;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);
        Card cardToChange = whoToChange == WhoToChange.Initiating ? context.InitiatingCard : context.OtherCard;

        if (decision.Decide(context))
        {
            cardToChange.attackPoints += advantageAmount;
        }

        else if (disadvantageAmount > 0)
        {
            cardToChange.attackPoints -= disadvantageAmount;
        }

        yield return new WaitForSeconds(postdelay);
    }

}
