using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesEffect : Effect
{
    private readonly CardArchetype _enemyArchetype;
    public SpawnEnemiesEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _enemyArchetype = blueprint.cardArchetype;
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);
        yield return _data.SpawnEnemiesByArchetype(_enemyArchetype, amount);
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "";
    }
}
