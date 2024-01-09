using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsEffect : Effect
{
    public int amount;
     
    public void Initialize(int amount)
    {
        this.amount = amount;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        yield return StartCoroutine(context.InitiatingCard.GainPoints(amount));

        SendLog();

        yield return new WaitForSeconds(postdelay);
    }

    private void SendLog()
    {
        string log = parentCard.name + "got " + "+ " + amount;
        events.AddLogEntry.Raise(this, log);
    }
}
