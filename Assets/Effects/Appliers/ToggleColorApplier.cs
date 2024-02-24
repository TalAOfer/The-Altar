using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleColorApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        CardColor currentColor = target.cardColor;
        CardColor newColor = currentColor is CardColor.Black ? CardColor.Red : CardColor.Black;
        target.cardColor = newColor;
        RaiseEffectAppliedEvent(target, amount);
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        CardColor newColor = target.cardColor;

        return "Change to " + newColor.ToString().ToLower();
    }
}
