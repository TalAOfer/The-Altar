using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveEffect : Effect
{
    public bool didMakeChoice;
    public override IEnumerator Apply(EffectContext context)
    {
        didMakeChoice = false;
        context.Events.WaitForActiveChoice.Raise(this, this);
        while(!didMakeChoice)
        {
            yield return null;
        }
    }

    public IEnumerator HandleChoice(Card chosenCard)
    {
        yield return StartCoroutine(ApplyEffectOnChoice(chosenCard));
        didMakeChoice = true;
    }

    public abstract IEnumerator ApplyEffectOnChoice(Card chosenCard);
}
