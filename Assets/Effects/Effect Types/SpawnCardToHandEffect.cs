using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCardToHandEffect : Effect
{
    private CardArchetype _archetype;

    public SpawnCardToHandEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, CardArchetype archetype) : base(blueprint, data, trigger, parentCard)
    {
        _archetype = archetype;
    }

    public override void ApplyEffect(Card target, int amount)
    {
        //Debug.Log("spawning");
        RaiseEffectAppliedEvent(target, amount);
        //for (int i = 0; i < amount; i++)
        //{
        //    actions.SpawnCardToHandByArchetype(archetype);
        //    yield return Tools.GetWait(0.1f);
        //}
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Spawn " + amount.ToString() + " " + Tools.GetCardNameByArchetype(_archetype, Affinity.Player);
    }
}
