using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectApplier : MonoBehaviour
{
    bool isConditional;
    Decision decision;
    public void BaseInitialize(bool isConditional, Decision decision)
    {
        this.isConditional = isConditional;
        this.decision = decision;
    }

    public IEnumerator Apply(Card target, RoomData data)
    {
        if (isConditional && !decision.Decide(target, data.GetOpponent(target))) yield break;

        yield return ApplyEffect(target);
    }

    public abstract IEnumerator ApplyEffect(Card target);
}
