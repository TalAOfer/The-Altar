using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEffect : Effect
{
    private bool didGetResponse;
    public override IEnumerator Apply()
    {
        didGetResponse = false;

        events.WaitForPlayerSelection.Raise(this, this);

        while (!didGetResponse)
        {
            yield return null;
        }
    }

    public IEnumerator HandleResponse(Component sender, object response)
    {
        yield return StartCoroutine(ApplyEffectOnResponse(sender, response));
        didGetResponse = true;
    }

    protected IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        List<Card> targetCards = response as List<Card>; 
        foreach (Card card in targetCards)
        {
            int amount = GetAmount(card);
            yield return StartCoroutine(applier.Apply(card, amount));
        }
    }
}