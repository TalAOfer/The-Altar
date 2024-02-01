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

    public override IEnumerator ApplyEffect(Card target)
    {
        target.cardColor = color;
        if (target.cardColor != color)
        {
            data.events.OnEffectApplied.Raise(this, new EffectIndication("Change to " + color.ToString(), target));
        }
        yield return null;
    }
}
