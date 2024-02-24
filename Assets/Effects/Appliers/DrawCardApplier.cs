using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        RaiseEffectAppliedEvent(target, amount);

        for (int i = 0; i < amount; i++)
        {
            actions.DrawCardToHand();
            yield return Tools.GetWait(0.1f);
        }
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Draw " + amount.ToString() + " cards";
    }
}
