using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCardToHandEffect : Effect
{
    private readonly CardArchetype _archetype;

    public SpawnCardToHandEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _archetype = blueprint.cardArchetype;
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);
        RaiseEffectAppliedEvent(target, amount);
        for (int i = 0; i < amount; i++)
        {
            _data.SpawnCardToHandByArchetype(_archetype);
            yield return Tools.GetWait(0.1f);
        }

        yield break;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "Spawn " + amount.ToString() + " " + Tools.GetCardNameByArchetype(_archetype, Affinity.Player);
    }
}
