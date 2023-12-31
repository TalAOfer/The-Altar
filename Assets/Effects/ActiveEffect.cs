using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class ActiveEffect : Effect
{
    public bool didGetResponse;

    public ResponseNeeded ResponseNeeded;
    public AllEvents Events;

    public void BaseInitialize(ResponseNeeded ResponseNeeded, AllEvents Events)
    {
        this.ResponseNeeded = ResponseNeeded;
        this.Events = Events;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        didGetResponse = false;
        GameEvent eventType = null;
        switch (ResponseNeeded)
        {
            case ResponseNeeded.None:
                Debug.Log("shouldn't be active");
                break;
            case ResponseNeeded.Choice:
                eventType = Events.WaitForActiveChoice;
                break;
            case ResponseNeeded.RandomFromHand:
                eventType = Events.GetRandomCardFromHand;
                break;
            case ResponseNeeded.RandomFromMap:
                break;
        }

        eventType.Raise(this, this);
        while(!didGetResponse)
        {
            yield return null;
        }
    }

    public IEnumerator HandleChoice(Card chosenCard)
    {
        yield return StartCoroutine(ApplyEffectOnChoice(chosenCard));
        didGetResponse = true;
    }

    protected abstract IEnumerator ApplyEffectOnChoice(Card chosenCard);
}
