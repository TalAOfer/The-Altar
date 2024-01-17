using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCardToHandApplier : EffectApplier
{
    public CardBlueprint blueprintToSummon;
    
    public void Initialize(CardBlueprint blueprintToSummon)
    {
        this.blueprintToSummon = blueprintToSummon;
    }
    public override IEnumerator ApplyEffect(Card target)
    {
        data.PlayerManager.SpawnCardToHandByBlueprint(blueprintToSummon);
        yield return null;
    }
}
