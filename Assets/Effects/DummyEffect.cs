using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEffect : Effect
{
    public override IEnumerator Apply(EffectContext context)
    {
        Debug.Log("Effect applied");
        yield return null;
    }
}
