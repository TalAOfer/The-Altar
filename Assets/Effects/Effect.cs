using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Effect : MonoBehaviour
{
    public EffectApplicationType effectApplicationType;
    protected float predelay;
    protected float postdelay;
    protected AllEvents events;
    public Card parentCard;
    public void BaseInitialize(EffectBlueprint blueprint, Card parentCard)
    {
        effectApplicationType = blueprint.applicationType;
        predelay = blueprint.predelay;
        postdelay = blueprint.postdelay;
        events = blueprint.events;

        this.parentCard = parentCard;
    }
    public abstract IEnumerator Apply(EffectContext context);
}

