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
        yield return null;
    }
}
