using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MetaPoolInstance
{
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<BlueprintPoolInstance> pools;

    public void Initialize(MetaPoolRecipe recipe)
    {
        pools?.Clear();
        pools = new();

        for (int i = 0; i < recipe.pools.Count; i++)
        {
            BlueprintPoolInstance pool = new();
            pool.Initialize(recipe.pools[i]);
            pools.Add(pool);
        }
    }
}
