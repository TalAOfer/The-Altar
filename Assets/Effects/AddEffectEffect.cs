using System.Collections;
using UnityEngine;

public class AddEffectEffect : Effect
{
    private EffectTrigger triggerType;
    private EffectBlueprint blueprintToAdd;
    private WhoToChange whoToAddTo;
    public void Initialize(EffectBlueprint blueprintToAdd, EffectTrigger triggerType, WhoToChange whoToAddTo)
    {
        this.blueprintToAdd = blueprintToAdd;
        this.triggerType = triggerType;
        this.whoToAddTo = whoToAddTo;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        Card cardToAddTo = whoToAddTo == WhoToChange.Initiating ? context.InitiatingCard : context.OtherCard;

        blueprintToAdd.SpawnEffect(triggerType, cardToAddTo);

        yield return new WaitForSeconds(postdelay);
    }
}
