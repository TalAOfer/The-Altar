using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
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
        SendLog();
        yield return StartCoroutine(context.InitiatingCard.ForceShapeshift(blueprint));
    }

    private void SendLog()
    {
        string log = parentCard.name + " force shapeshifted into " + blueprint.name;
        events.AddLogEntry.Raise(this, log);
    }
}
