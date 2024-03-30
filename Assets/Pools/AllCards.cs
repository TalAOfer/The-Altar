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

    public MetaPoolRecipe playerPool;

#if UNITY_EDITOR

    [Button]
    public void SortCards()
    {
        LoadAllCardBlueprints();

        playerPool.ResetPools();

        foreach (var card in allCards)
        {
            if (card.isDefault || card.Affinity is Affinity.Enemy) continue;
            BlueprintPoolInstance pointPool = playerPool.pools[card.Archetype.points];
            List<CardBlueprint> correctPool = card.Archetype.color is CardColor.Black ? pointPool.black : pointPool.red;
            correctPool.Add(card);
        }

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
