using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public EffectApplier applier;
    public AllEvents events;
    public EffectApplicationType effectApplicationType;
    protected float predelay;
    protected float postdelay;
    public Card parentCard;
    public void BaseInitialize(EffectApplier applier, Card parentCard, EffectBlueprint blueprint)
    {
        this.applier = applier;
        this.parentCard = parentCard;

        effectApplicationType = blueprint.applicationType;
        predelay = blueprint.predelay;
        postdelay = blueprint.postdelay;
        events = blueprint.events;
    }

    public IEnumerator Trigger(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        yield return Apply(context);

        yield return new WaitForSeconds(postdelay);
    }

    public abstract IEnumerator Apply(EffectContext context);
}
