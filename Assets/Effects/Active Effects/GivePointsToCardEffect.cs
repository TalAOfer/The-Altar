using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class GivePointsToCardEffect : ActiveEffect
{
    public int amount;
    public void Initialize(int amount)
    {
        this.amount = amount;
    }

    public override void SendEvent()
    {
        events.GetRandomCardFromHand.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Card chosenCard)
    {
        yield return new WaitForSeconds(predelay);

        yield return StartCoroutine(chosenCard.GainPoints(amount, true));

        yield return new WaitForSeconds(postdelay);
    }
}
