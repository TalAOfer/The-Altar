#if UNITY_EDITOR

using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public class EffectUsageFinder : OdinEditorWindow
{
    [MenuItem("Tools/Effect Usage Finder")]
    private static void OpenWindow()
    {
        GetWindow<EffectUsageFinder>().Show();
    }

    [Required]
    public EffectBlueprint SelectedEffect;

    [ReadOnly]
    public List<CardBlueprint> CardsUsingEffect;

    [ReadOnly]
    public List<EffectBlueprint> EffectsUsingEffect;

    [Button(ButtonSizes.Medium)]
    private void FindUsage()
    {
        CardsUsingEffect = new List<CardBlueprint>();
        EffectsUsingEffect = new List<EffectBlueprint>();
        var allCards = AssetDatabase.FindAssets("t:CardBlueprint");
        var allEffects = AssetDatabase.FindAssets("t:EffectBlueprint");

        foreach (var guid in allCards)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var card = AssetDatabase.LoadAssetAtPath<CardBlueprint>(path);
            if (IsEffectUsedInCard(card))
            {
                CardsUsingEffect.Add(card);
            }
        }

        foreach (var guid in allEffects)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var effect = AssetDatabase.LoadAssetAtPath<EffectBlueprint>(path);
            if (effect.effectBlueprint == SelectedEffect)
            {
                EffectsUsingEffect.Add(effect);
            }
        }
    }

    private bool IsEffectUsedInCard(CardBlueprint card)
    {
        var allLists = new List<List<EffectBlueprint>> { card.StartOfTurn, card.StartOfBattle, card.Support, card.BeforeAttacking, card.OnDeath, card.OnGlobalDeath, card.OnSurvive, card.Bloodthirst, card.Meditate };
        return allLists.Any(list => list.Contains(SelectedEffect));
    }
}

#endif