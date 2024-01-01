using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class DrawCardEffect : ActiveEffect
{
    protected override IEnumerator ApplyEffectOnResponse(Card chosenCard)
    {
        yield return null;
    }

    public override void SendEvent()
    {
        events.DrawCardToHand.Raise(this, this);
    }
}