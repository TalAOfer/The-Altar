using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBase : MonoBehaviour
{
    public int Index { get; private set; }
    public Affinity Affinity { get; private set; }    
    public CardArchetype Archetype { get; private set; }
    public CardBlueprint Mask {  get; private set; } 

    public CardBase(int Index, CardBlueprint Mask, bool InitializeFromMask, Affinity Affinity = 0, CardArchetype Archetype = null) 
    {
        this.Index = Index;
        this.Mask = Mask;

        if (InitializeFromMask)
        {
            this.Affinity = Mask.Affinity;
            this.Archetype = Mask.Archetype;
        } 

        else
        {
            this.Affinity = Affinity;
            this.Archetype = Archetype;
        }

    }
}
