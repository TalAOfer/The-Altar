using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedShapeshiftEffect : Effect
{
    public CardBlueprint blueprint;
    public void Initialize(CardBlueprint blueprint)
    {
        this.blueprint = blueprint;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        yield return StartCoroutine(context.InitiatingCard.ForceShapeshift(blueprint));
    }
}
