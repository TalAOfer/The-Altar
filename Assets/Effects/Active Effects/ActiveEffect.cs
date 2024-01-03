using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ActiveEffect : Effect
{
    public bool didGetResponse;
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        didGetResponse = false;

        SendEvent();

        while (!didGetResponse)
        {
            yield return null;
        }

        yield return new WaitForSeconds(postdelay);
    }

    public abstract void SendEvent();

    public IEnumerator HandleResponse(Component sender, object response)
    {
        yield return StartCoroutine(ApplyEffectOnResponse(sender, response));
        didGetResponse = true;
    }

    protected abstract IEnumerator ApplyEffectOnResponse(Component sender, object response);
}
