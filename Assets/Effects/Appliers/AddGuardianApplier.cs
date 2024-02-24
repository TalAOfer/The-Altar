using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianApplier : EffectApplier
{
    private GuardianType guardianType;
    private EffectApplicationType applicationType;

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        target.guardians.Add(new Guardian(guardianType, parentCard, applicationType));
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        throw new System.NotImplementedException();
    }

    public void Initialize(GuardianType guardianType, EffectApplicationType applicationType)
    {
        this.guardianType = guardianType;
        this.applicationType = applicationType;
    }


}
