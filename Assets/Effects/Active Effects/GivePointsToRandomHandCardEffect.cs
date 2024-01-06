using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        events.GetAllCardsFromHand.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        List<Card> cardsInHand;

        
        switch (response)
        {
            case List<Card> cards:
                cardsInHand = new List<Card>(cards);
                break;
            default:
                Debug.LogError("got wrong data type from " + sender.name);
                yield break;
        }

        //So that it doesn't apply it on itself
        cardsInHand.Remove(parentCard);
        int rand = Random.Range(0, cardsInHand.Count);
        Card randCard = cardsInHand[rand];
        yield return StartCoroutine(randCard.GainPoints(amount));

        yield return new WaitForSeconds(postdelay);
    }
}
