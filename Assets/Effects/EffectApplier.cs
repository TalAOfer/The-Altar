using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectApplier : MonoBehaviour
{
    bool isConditional;
    Decision decision;
    public IEnumerator Apply(ApplierContext context)
    {
        if (isConditional && !decision.Decide(context)) yield break;

        yield return ApplyEffect(context);
    }

    public abstract IEnumerator ApplyEffect(ApplierContext context);
}
