using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleColorApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target)
    {
        CardColor currentColor = target.cardColor;
        CardColor newColor = currentColor is CardColor.Black ? CardColor.Red : CardColor.Black;
        target.cardColor = newColor;
        data.events.OnEffectApplied.Raise(this, new EffectIndication("Color change", target));
        yield return null;
    }
}
