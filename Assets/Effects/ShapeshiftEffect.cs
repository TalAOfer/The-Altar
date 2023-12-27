using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeshiftEffect : Effect
{
    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        yield return StartCoroutine(context.InitiatingCard.Shapeshift());

        yield return new WaitForSeconds(postdelay);
    }
}
