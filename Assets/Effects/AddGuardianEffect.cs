using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianEffect : Effect
{
    private GuardianType guardianType;
    private EffectApplicationType applicationType;

    public void Initialize(GuardianType guardianType, EffectApplicationType applicationType)
    {
        this.guardianType = guardianType;
        this.applicationType = applicationType;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay); 

        context.OtherCard.guardians.Add(new Guardian(guardianType, context.InitiatingCard, applicationType));

        yield return new WaitForSeconds(postdelay);
    }
}
