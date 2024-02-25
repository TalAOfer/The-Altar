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

    public BattleBlueprint battle;

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

