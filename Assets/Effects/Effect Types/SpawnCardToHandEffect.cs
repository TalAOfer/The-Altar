using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCardToHandEffect : Effect
{
    private readonly CardArchetype _archetype;

    public SpawnCardToHandEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, CardArchetype archetype) : base(blueprint, data, trigger, parentCard)
    {
        _archetype = archetype;
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        RaiseEffectAppliedEvent(target, amount);
        for (int i = 0; i < amount; i++)
        {
            data.SpawnCardToHandByArchetype(_archetype);
            yield return Tools.GetWait(0.1f);
        }

        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Spawn " + amount.ToString() + " " + Tools.GetCardNameByArchetype(_archetype, Affinity.Player);
    }
}
