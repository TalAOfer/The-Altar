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
    protected override IEnumerator ApplyEffectOnChoice(Card chosenCard)
    {
        yield return new WaitForSeconds(predelay);

        yield return StartCoroutine(chosenCard.GainPoints(amount));

        yield return new WaitForSeconds(postdelay);
    }
}
