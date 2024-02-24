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
        RaiseEffectAppliedEvent(target, amount);
        for (int i = 0; i < amount; i++)
        {
            actions.SpawnCardToHandByArchetype(archetype);
            yield return Tools.GetWait(0.1f);
        }
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Spawn " + amount.ToString() + " " + Tools.GetCardNameByArchetype(archetype, CardOwner.Player);
    }
}
