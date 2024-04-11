using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PredeterminedBattleBlueprint : IBattleBlueprint
{
    public CodexBlueprint defaultCodex;
    public CodexBlueprint fullCodex;
    public CodexProgressionMap ProgressionMap;
    private Codex codex;

    [ValueDropdown("AvailableCards")]
    [Title("Enemies")]
    public List<CardBlueprint> cards = new();
    public List<CardBlueprint> GetCards()
    {
        return cards;
    }

    private static CardBlueprint DrawCardName(Rect rect, CardBlueprint value)
    {
        value = (CardBlueprint)EditorGUI.ObjectField(rect, value, typeof(CardBlueprint), false);

        if (value != null)
        {
            Rect labelRect = new(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            Color prevColor = GUI.color;
            GUI.color = Color.clear;
            EditorGUI.LabelField(labelRect, value.name);
            GUI.color = prevColor;
        }

        return value;
    }

    private IEnumerable<CardBlueprint> AvailableCards
    {
        get
        {
            if (codex == null || !codex.red.Any() || !codex.black.Any()) 
            {
                InitializeCodex();
            }

            return codex.black.Concat(codex.red);
        }
    }

    private void InitializeCodex()
    {
        codex = new Codex(defaultCodex);
        foreach (CardArchetype archetype in ProgressionMap.OpenMasksArchetypes)
        {
            CardBlueprint cardBlueprint = fullCodex.GetBlueprintByArchetype(archetype);
            codex.OverrideCard(cardBlueprint);
        }
    }

}

public interface IBattleBlueprint
{
    public List<CardBlueprint> GetCards();
}

