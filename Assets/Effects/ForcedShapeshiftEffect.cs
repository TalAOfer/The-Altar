using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedShapeshiftEffect : Effect
{
    public CardBlueprint blueprint;
    public ShapeshiftType shapeshiftType;
    public void Initialize(CardBlueprint blueprint, ShapeshiftType shapeshiftType)
    {
        this.blueprint = blueprint;
        this.shapeshiftType = shapeshiftType;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        yield return StartCoroutine(context.InitiatingCard.ForceShapeshift(blueprint, shapeshiftType));
    }
}
