using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Effect : MonoBehaviour
{
    public float predelay;
    public float postdelay;
    public void InitializeDelay(float predelay, float postdelay)
    {
        this.predelay = predelay;
        this.postdelay = postdelay;
    }
    public abstract IEnumerator Apply(EffectContext context);
}

