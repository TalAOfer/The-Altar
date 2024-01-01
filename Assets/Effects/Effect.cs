using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Effect : MonoBehaviour
{
    public float predelay;
    public float postdelay;
    public AllEvents events;
    public void BaseInitialize(float predelay, float postdelay, AllEvents events)
    {
        this.predelay = predelay;
        this.postdelay = postdelay;
        this.events = events;
    }
    public abstract IEnumerator Apply(EffectContext context);
}

