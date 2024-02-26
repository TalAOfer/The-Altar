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
#if UNITY_EDITOR

    [Button]
    public void SortCards()
    {
        LoadAllCardBlueprints();

        enemyPool.ResetPools();
        playerPool.ResetPools();

        foreach (var card in allCards)
        {
            if (card.isDefault) continue;
            MetaPoolRecipe metaPool = card.Affinity is Affinity.Player ? playerPool : enemyPool;
            BlueprintPoolInstance pointPool = metaPool.pools[card.Archetype.points];
            List<CardBlueprint> correctPool = card.Archetype.color is CardColor.Black ? pointPool.black : pointPool.red;
            correctPool.Add(card);
        }

        EditorUtility.SetDirty(enemyPool);
        EditorUtility.SetDirty(playerPool);
        AssetDatabase.SaveAssets();
    }

    private void LoadAllCardBlueprints()
    {
        allCards.Clear(); // Optional: Clear the list if you don't want duplicates or already added assets.
        string[] guids = AssetDatabase.FindAssets("t:CardBlueprint");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CardBlueprint asset = AssetDatabase.LoadAssetAtPath<CardBlueprint>(assetPath);
            if (asset != null)
            {
                allCards.Add(asset);
            }
        }
    }
#endif

}
