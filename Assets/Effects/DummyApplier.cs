using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyApplier : EffectApplier
{
    public override IEnumerator ApplyEffect(ApplierContext context)
    {
        Debug.Log("Happened");
        yield return null;
    }
}
