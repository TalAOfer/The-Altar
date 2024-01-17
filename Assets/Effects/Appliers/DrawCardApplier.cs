using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target)
    {
        data.PlayerManager.DrawCardToHand();
        yield return null;
    }
}
