using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEffect : Effect
{
    public override IEnumerator Apply(EffectContext context)
    {
        string log = "happened on " + parentCard.name;
        events.AddLogEntry.Raise(this, log);
        yield return null;
    }
}
