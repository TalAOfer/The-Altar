using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianEffect : Effect
{
    private GuardianType _guardianType;
    private EffectApplicationType _applicationType;

    public AddGuardianEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, GuardianType guardianType, EffectApplicationType applicationType) : base(blueprint, data, trigger, parentCard)
    {
        _guardianType = guardianType;
        _applicationType = applicationType;
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        target.guardians.Add(new Guardian(_guardianType, parentCard, _applicationType));
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        throw new System.NotImplementedException();
    }

}
