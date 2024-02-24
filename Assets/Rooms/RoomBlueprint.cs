using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class RoomBlueprint
{
    [TableColumnWidth(80, Resizable = false)]
    [GUIColor("GetColorForRoomType")]
    public RoomType roomType;

    #region Battle
    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public int difficulty = 3;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public bool predetermineDeck;
    
    [FoldoutGroup("Room Data")]
    [ShowIf("@ShouldShowDeckBlueprint()")]
    public Deck deck;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public bool isTutorial;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public CodexBlueprint codex;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    [ValueDropdown("AvailableCards")]
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

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public GridPlacement_3 placement;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    [TableList(ShowIndexLabels = true)]
    public List<EnemySpawn> enemies;

    [FoldoutGroup("Room Data")]
    [OnInspectorGUI]
    private void ValidateSpawns()
    {
        if (enemies.GroupBy(x => x.Placement).Any(g => g.Count() > 1))
        {
            Sirenix.Utilities.Editor.SirenixEditorGUI.ErrorMessageBox("Some of the EnemySpawns have the same placement. Please ensure all EnemySpawns have unique placements.");
        }
    }

    [FoldoutGroup("Room Data")]
    [Button]
    public void AddToEnemies()
    {
        enemies.Add(new EnemySpawn(card, placement));
    }

    #endregion

    #region Card Picking

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.CardPicking)]
    public int amountOfOptions;

    #endregion

    #region Playtest

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.PlaytestCardGain)]
    public Affinity affinity;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.PlaytestCardGain)]
    public CardBlueprint cardBlueprint;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.PlaytestCardGain)]
    public bool shouldShowExtraText;

    #endregion

    private bool ShouldShowDeckBlueprint()
    {
        return roomType is RoomType.Battle && predetermineDeck;
    }

    private bool ShouldHideEnemyMinmax()
    {
        return roomType is not RoomType.CardPicking && (roomType is RoomType.PlaytestCardGain);
    }

    private Color GetColorForRoomType()
    {
        Color color = Color.white;
        switch (roomType)
        {
            case RoomType.Battle:
                //Muted red
                color = !isTutorial ? new Color(0.8f, 0.3f, 0.3f, 1.0f) : color = new Color(0.7f, 0.3f, 0.4f, 1.0f);
                break;
            case RoomType.CardPicking:
                //Muted green
                color = new Color(0.3f, 0.6f, 0.3f, 1.0f);
                break;
            case RoomType.PlaytestCardGain:
                //Muted blue
                color = new Color(0.3f, 0.4f, 0.8f, 1.0f);
                break;
        }
        //(0.9f, 0.9f, 0.5f, 1.0f) muted yellow for future use

        return color;
    }
}

