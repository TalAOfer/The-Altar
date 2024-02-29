using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Blueprints/Battle")]
public class BattleBlueprint : SerializedScriptableObject
{
    public CodexBlueprint codex;

    [TableMatrix(HorizontalTitle = "Enemy Grid", DrawElementMethod = "DrawCardName", Transpose = true)]
    [ShowInInspector]
    [ReadOnly]
    public CardBlueprint[,] cardGrid = new CardBlueprint[3, 3];
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

    [ValueDropdown("AvailableCards")]
    [BoxGroup()]
    public CardBlueprint card;

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

    [BoxGroup()]
    public GridPlacement_3 placement;

    [OnInspectorGUI]
    private void ValidateSpawns()
    {
        if (IsOccupied())
        {
            Sirenix.Utilities.Editor.SirenixEditorGUI.ErrorMessageBox("Slot is occupied.");
        }
    }

    private bool IsOccupied()
    {
        var (row, col) = GetGridSlotIndices();
        return cardGrid[row, col] != null;
    }

    private (int row, int col) GetGridSlotIndices()
    {
        int row = (int)placement / 3;
        int col = (int)placement % 3;
        return (row, col);
    }

    [BoxGroup()]
    [Button("Add")]
    public void AddToEnemies()
    {
        if (!IsOccupied())
        {
            var (row, col) = GetGridSlotIndices();
            //Debug.Log($"Adding - Row: {row}, Col: {col}");
            cardGrid[row, col] = card;
        }
    }

    [BoxGroup()]
    [Button("Remove")]
    public void RemoveFromGrid()
    {
        if (IsOccupied())
        {
            var (row, col) = GetGridSlotIndices();
            cardGrid[row, col] = null;
        }
    }
}
public enum GridPlacement_3
{
    TopLeft, TopMiddle, TopRight,
    CenterLeft, CenterMiddle, CenterRight,
    BottomLeft, BottomMiddle, BottomRight,
}