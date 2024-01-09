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

        SendLog(context.OtherCard);

        yield return new WaitForSeconds(postdelay);
    }

    private void SendLog(Card otherCard)
    {
        string log = parentCard.name + " added " + guardianType.ToString() + " to " + otherCard.name + "'s guardians";
        events.AddLogEntry.Raise(this, log);
    }
}
