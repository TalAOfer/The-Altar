using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class MetaPoolInstance
{
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<BlueprintPoolInstance> pools;

    public CardBlueprint GetRandomCardByPoints(int minPoints, int maxPoints)
    {
        int attempts = 0;
        const int maxAttempts = 100; // Prevent infinite loops

        while (attempts < maxAttempts)
        {
            int currentPoints = UnityEngine.Random.Range(minPoints, maxPoints + 1);

            // Ensure the currentPoints index is within the range of the pools list
            if (currentPoints >= 0 && currentPoints < pools.Count)
            {
                BlueprintPoolInstance pointPool = pools[currentPoints];
                CardColor poolColor = (CardColor)UnityEngine.Random.Range(0, 2);
                List<CardBlueprint> pool = poolColor == CardColor.Black ? pointPool.black : pointPool.red;

                if (pool.Count > 0)
                {
                    int randIndexInPool = UnityEngine.Random.Range(0, pool.Count);
                    CardBlueprint drawnBlueprint = pool[randIndexInPool];
                    pool.RemoveAt(randIndexInPool); // Remove the blueprint from the pool
                    return drawnBlueprint;
                }
            }

            attempts++;
        }

        throw new System.InvalidOperationException("Unable to find a card in the specified point range after " + maxAttempts + " attempts.");
    }

    public void ReturnBlueprintToPool(CardBlueprint blueprint)
    {
        BlueprintPoolInstance pointPool = pools[blueprint.Archetype.points];
        CardColor poolColor = blueprint.Archetype.color;
        List<CardBlueprint> pool = poolColor is CardColor.Black ? pointPool.black : pointPool.red;

        pool.Add(blueprint);
    }

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
