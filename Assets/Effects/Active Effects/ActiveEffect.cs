using UnityEngine;
using System.Collections;

public abstract class ActiveEffect : Effect
{
    public bool didGetResponse;
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        didGetResponse = false;

        SendEvent();

        while(!didGetResponse)
        {
            yield return null;
        }

        yield return new WaitForSeconds(postdelay);
    }

    public abstract void SendEvent();

    public IEnumerator HandleResponse(Card chosenCard)
    {
        yield return StartCoroutine(ApplyEffectOnResponse(chosenCard));
        didGetResponse = true;
    }

    protected abstract IEnumerator ApplyEffectOnResponse(Card chosenCard);
}
