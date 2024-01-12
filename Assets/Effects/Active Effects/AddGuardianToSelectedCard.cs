using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianToSelectedCard : ActiveEffect
{
    public override void SendEvent()
    {
        events.WaitForPlayerSelection.Raise(this, this);
    }

    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        SelectManager selectManager = sender as SelectManager;

        yield return StartCoroutine(selectManager.BringBackToDefault());

        Card chosenCard = (Card)response;
        string log = parentCard.name + " chose " + chosenCard.name;
        events.AddLogEntry.Raise(this, log);
        yield return null;
    }
}
