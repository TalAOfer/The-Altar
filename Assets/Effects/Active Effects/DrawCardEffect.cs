using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class DrawCardEffect : ActiveEffect
{
    protected override IEnumerator ApplyEffectOnResponse(Component sender, object response)
    {
        yield return null;
    }

    public override void SendEvent()
    {
        //string log = parentCard.name + " added a card to player's hand";
        //events.AddLogEntry.Raise(this, log);
        events.DrawCardToHand.Raise(this, this);
    }
}