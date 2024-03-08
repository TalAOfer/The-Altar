using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesEffect : Effect
{
    private readonly CardArchetype _enemyArchetype;
    public SpawnEnemiesEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, CardArchetype enemyArchetype) : base(blueprint, data, trigger, parentCard)
    {
        _enemyArchetype = enemyArchetype;
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        yield return data.SpawnEnemiesByArchetype(_enemyArchetype, amount);
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "";
    }
}
