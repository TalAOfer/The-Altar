using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class GivePointsToRandomHandCardEffect : ActiveEffect
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

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        List<Card> cardsToChange;
        switch (response)
        {
            case List<Card> cards:
                cardsToChange = new List<Card>(cards);
                break;
            default:
                Debug.LogError("got wrong data type from " + sender.name);
                yield break;
        }

        yield return new WaitForSeconds(predelay);

        foreach (Card card in cardsToChange)
        {
            yield return StartCoroutine(card.GainPoints(amount));
            yield return StartCoroutine(card.HandleShapeshift());
        }

        yield return new WaitForSeconds(postdelay);
    }
}
