using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEffect : Effect
{
    private bool didGetResponse;
    public GameEvent asker;
    public override IEnumerator Apply(EffectContext context)
    {
        didGetResponse = false;

        asker.Raise(this, this);

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
            yield return StartCoroutine(applier.Apply(new ApplierContext(card, parentCard)));
        }
    }
}