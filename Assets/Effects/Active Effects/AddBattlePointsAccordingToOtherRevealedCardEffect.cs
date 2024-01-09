using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBattlePointsAccordingToOtherRevealedCardEffect : ActiveEffect
{
    public override void SendEvent()
    {
        events.GetRevealedEnemyCards.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        yield return new WaitForSeconds(predelay);

        List<Card> activeEnemies = (List<Card>)response;
        Card otherRevealedCard = null;

        foreach (Card card in activeEnemies)
        {
            if (parentCard != card)
            {
                otherRevealedCard = card;
            }
        }

        int buffAmount = Tools.MultAndRoundUp(otherRevealedCard.points, 0.5f);
        parentCard.attackPointsModifiers.Add(new BattlePointModifier(ModifierType.Addition, buffAmount));

        //SendLog(parentCard, buffAmount);

        yield return null;
    }

    private void SendLog(Card thisCard, int buffAmount)
    {
        string log = thisCard.name + " got +" + buffAmount.ToString() + "attack points from other revealed card";
        events.AddLogEntry.Raise(this, log);
    }
}
