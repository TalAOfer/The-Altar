using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount();
        Debug.Log("drawing");
        for (int i = 0; i < amount; i++)
        {
            data.PlayerManager.DrawCardToHand();
        }
        yield return null;
    }
}
