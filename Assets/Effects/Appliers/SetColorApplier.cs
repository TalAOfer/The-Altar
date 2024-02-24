using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorApplier : EffectApplier
{
    public CardColor color;

    public void Initialize(CardColor color)
    {
        this.color = color;
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        target.cardColor = color;
        RaiseEffectAppliedEvent(target, amount);
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Change to " + color.ToString();
    }
}
