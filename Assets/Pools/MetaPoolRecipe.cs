using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "AllPools")]
public class MetaPoolRecipe : ScriptableObject
{
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<BlueprintPoolInstance> pools;

    public void ResetPools()
    {
        pools?.Clear();
        pools = new List<BlueprintPoolInstance>();

        for (int i = 0; i <= 10; i++)
        {
            pools.Add(new BlueprintPoolInstance());
        }
    }
}
