using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Blueprints/Battle")]
public class BattleBlueprint : ScriptableObject
{
    public CodexBlueprint codex;

    [ValueDropdown("AvailableCards")]
    public List<CardBlueprint> cards = new();
    private static CardBlueprint DrawCardName(Rect rect, CardBlueprint value)
    {
        // Start by allowing object selection and assignment
        value = (CardBlueprint)EditorGUI.ObjectField(rect, value, typeof(CardBlueprint), false);

        // Draw additional info only if the CardBlueprint is not null
        if (value != null)
        {
            // Create a rect for the name label
            Rect labelRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            // Set GUI color to transparent to only draw text without the field background
            Color prevColor = GUI.color;
            GUI.color = Color.clear;
            EditorGUI.LabelField(labelRect, value.name);
            GUI.color = prevColor;
        }

        // Return the possibly new value assigned in the ObjectField
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
