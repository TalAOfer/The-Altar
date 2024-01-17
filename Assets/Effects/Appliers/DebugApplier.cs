using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(Card target)
    {
        Debug.Log("Happened on " + target.name);
        yield return null;
    }
}
