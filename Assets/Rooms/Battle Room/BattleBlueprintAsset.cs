using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Battle")]
public class BattleBlueprintAsset : ScriptableObject
{
    public BattleBlueprint Value;
}

[Serializable]
public class BattleBlueprint : IBattleBlueprint
{
    public CodexBlueprint codex;
    public int Difficulty;

    [ValueDropdown("AvailableCards")]
    public List<CardBlueprint> cards = new();

    public List<CardBlueprint> GetCards()
    {
        return cards;
    }

    public BattleBlueprint(BattleBlueprint source)
    {
        codex = source.codex; 
        cards = new List<CardBlueprint>(source.cards);
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
            if (codex != null)
            {
                // Combine lists if you want to include multiple categories
                return codex.black.Concat(codex.red);
            }
            return new List<CardBlueprint>();
        }
    }
}
