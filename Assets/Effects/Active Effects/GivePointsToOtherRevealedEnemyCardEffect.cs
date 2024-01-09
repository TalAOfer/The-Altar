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

        SendLog(otherRevealedCard, 2);

        yield return new WaitForSeconds(postdelay);
    }

    private void SendLog(Card otherCard, int amount)
    {
        string log = otherCard.name + " got +" + amount + " from " + parentCard.name;
        events.AddLogEntry.Raise(this, log);
    }
}
