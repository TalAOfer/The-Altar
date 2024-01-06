using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePointsToOtherRevealedEnemyCardEffect : ActiveEffect
{
    public override void SendEvent()
    {
        events.GetRevealedEnemyCards.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        yield return new WaitForSeconds(predelay);

        List<Card> activeEnemies = (List<Card>) response;
        Card otherRevealedCard = null;

        foreach (Card card in activeEnemies)
        {
            if (parentCard != card)
            {
                otherRevealedCard = card;
            }
        }


        yield return StartCoroutine(otherRevealedCard.GainPoints(2));
        yield return StartCoroutine(otherRevealedCard.HandleShapeshift());


        yield return new WaitForSeconds(postdelay);
    }
}
