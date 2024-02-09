using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        data.events.OnEffectApplied.Raise(this, new EffectIndication("Draw " + amount.ToString() + " cards", parentCard));

        for (int i = 0; i < amount; i++)
        {
            data.PlayerManager.DrawCardToHand();
            yield return Tools.GetWait(0.1f);
        }
        yield return null;
    }
}
