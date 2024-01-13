using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

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
            pool.InitializeAsPool(recipe.pools[i]);
            pools.Add(pool);
        }
    }
}
