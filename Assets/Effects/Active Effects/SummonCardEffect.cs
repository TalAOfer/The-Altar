using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonCardEffect : ActiveEffect
{
    public CardBlueprint blueprintOfCardToSpawn;

    public void Initialize(CardBlueprint blueprintOfCardToSpawn)
    {
        this.blueprintOfCardToSpawn = blueprintOfCardToSpawn;
    }
    protected override IEnumerator ApplyEffectOnResponse(Card chosenCard)
    {
        yield return null;
    }

    public override void SendEvent()
    {
        events.SpawnCardToHand.Raise(this, blueprintOfCardToSpawn);
    }
}
