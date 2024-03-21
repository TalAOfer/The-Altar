using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianEffect : Effect
{
    private GuardianType _guardianType;
    private bool _isPersistent;

    public AddGuardianEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _guardianType = blueprint.guardianType;
        _isPersistent = blueprint.IsPersistent;
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        target.guardians.Add(new Guardian(_guardianType, ParentCard, _isPersistent));
        yield break;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        throw new System.NotImplementedException();
    }

}
