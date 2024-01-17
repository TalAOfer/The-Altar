using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianApplier : EffectApplier
{
    private GuardianType guardianType;
    private EffectApplicationType applicationType;

    public override IEnumerator ApplyEffect(Card target)
    {
        target.guardians.Add(new Guardian(guardianType, parentCard, applicationType));
        yield return null;
    }

    public void Initialize(GuardianType guardianType, EffectApplicationType applicationType)
    {
        this.guardianType = guardianType;
        this.applicationType = applicationType;
    }


}
