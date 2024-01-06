using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ChangeColorToAllRevealedEnemies : ActiveEffect
{
    public override void SendEvent()
    {
        events.GetRevealedEnemyCards.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        yield return new WaitForSeconds(predelay);

        List<Card> activeEnemies = (List<Card>)response;

        foreach (Card card in activeEnemies)
        {
            ToggleCardColor(card);
        }

        yield return new WaitForSeconds(postdelay);
    }

    private void ToggleCardColor(Card card)
    {
        if (card.cardColor == CardColor.Black)
        {
            card.cardColor = CardColor.Red;
        }
        else
        {
            card.cardColor = CardColor.Black;
        }
    }
}
