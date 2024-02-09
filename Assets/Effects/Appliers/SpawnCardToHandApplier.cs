using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCardToHandApplier : EffectApplier
{
    public CardArchetype archetype;
    
    public void Initialize(CardArchetype archetype)
    {
        this.archetype = archetype;
    }
    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        //Debug.Log("spawning");
        data.events.OnEffectApplied.Raise(this, new EffectIndication("Spawn " + amount.ToString() + " " + Tools.GetCardNameByArchetype(archetype, CardOwner.Player), parentCard));
        for (int i = 0; i < amount; i++)
        {
            data.PlayerManager.SpawnCardToHandByArchetype(archetype);
            yield return Tools.GetWait(0.1f);
        }
        yield return null;
    }
}
