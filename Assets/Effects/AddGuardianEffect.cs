using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGuardianEffect : Effect
{
    private GuardianType guardianType;

    public void Initialize(GuardianType guardianType)
    {
        this.guardianType = guardianType;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay); 

        context.OtherCard.guardians.Add(new Guardian(guardianType, context.InitiatingCard));

        yield return new WaitForSeconds(postdelay);
    }
}
