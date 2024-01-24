using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[CreateAssetMenu(menuName = "AllCards")]
public class AllCards : ScriptableObject
{
    public List<CardBlueprint> allCards;

    public MetaPoolRecipe enemyPool;
    public MetaPoolRecipe playerPool;

    [Button]
    public void SortCards()
    {
        enemyPool.ResetPools();
        playerPool.ResetPools();

        foreach (var card in allCards)
        {
            if (card.isDefault) continue;
            MetaPoolRecipe metaPool = card.cardOwner is CardOwner.Player ? playerPool : enemyPool;
            BlueprintPoolInstance pointPool = metaPool.pools[card.defaultPoints];
            List<CardBlueprint> correctPool = card.cardColor is CardColor.Black ? pointPool.black : pointPool.red;
            correctPool.Add(card);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(enemyPool);
        EditorUtility.SetDirty(playerPool);
        AssetDatabase.SaveAssets();
#endif
    }
}
