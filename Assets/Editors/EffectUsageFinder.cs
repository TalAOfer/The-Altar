#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class EffectUsageFinder : OdinEditorWindow
{
    //[MenuItem("Tools/Effect Usage Finder")]
    //private static void OpenWindow()
    //{
    //    GetWindow<EffectUsageFinder>().Show();
    //}

    //[Required]
    //public EffectBlueprintAsset SelectedEffect;

    //[ReadOnly]
    //public List<CardBlueprint> CardsUsingEffect;

    //[ReadOnly]
    //public List<EffectBlueprint> EffectsUsingEffect;

    //[Button(ButtonSizes.Medium)]
    //private void FindUsage()
    //{
    //    CardsUsingEffect = new List<CardBlueprint>();
    //    EffectsUsingEffect = new List<EffectBlueprint>();
    //    var allCards = AssetDatabase.FindAssets("t:CardBlueprint");
    //    var allEffects = AssetDatabase.FindAssets("t:EffectBlueprint");

    //    foreach (var guid in allCards)
    //    {
    //        var path = AssetDatabase.GUIDToAssetPath(guid);
    //        var card = AssetDatabase.LoadAssetAtPath<CardBlueprint>(path);
    //        if (IsEffectUsedInCard(card, SelectedEffect))
    //        {
    //            CardsUsingEffect.Add(card);
    //        }
    //    }

    //    foreach (var guid in allEffects)
    //    {
    //        var path = AssetDatabase.GUIDToAssetPath(guid);
    //        var effect = AssetDatabase.LoadAssetAtPath<EffectBlueprintAsset>(path);
    //        if (effect.blueprint == SelectedEffect)
    //        {
    //            EffectsUsingEffect.Add(effect);
    //        }
    //    }
    //}

    //private bool IsEffectUsedInCard(CardBlueprint card, EffectBlueprint selectedEffect)
    //{
    //    // Iterate over each list of EffectBlueprints in the dictionary's values
    //    foreach (var effectList in card.Effects.Values)
    //    {
    //        // Check if the current list contains the SelectedEffect
    //        if (effectList.Contains(selectedEffect))
    //        {
    //            return true; // Found the selected effect
    //        }
    //    }
    //    return false; // Selected effect not found in any list
    //}
}

#endif