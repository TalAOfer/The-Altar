using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Effect : MonoBehaviour
{
    public EffectApplicationType effectApplicationType;
    public float predelay;
    public float postdelay;
    public AllEvents events;
    public void BaseInitialize(EffectBlueprint blueprint)
    {
        effectApplicationType = blueprint.applicationType;
        predelay = blueprint.predelay;
        postdelay = blueprint.postdelay;
        events = blueprint.events;
    }
    public abstract IEnumerator Apply(EffectContext context);
}

