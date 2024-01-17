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
    public override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount();
        for (int i = 0; i < amount; i++)
        {
            data.PlayerManager.SpawnCardToHandByArchetype(archetype);
        }
        yield return null;
    }
}
