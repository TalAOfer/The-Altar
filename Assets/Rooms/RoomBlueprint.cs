using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

[Serializable]
public class RoomBlueprint
{
    [TableColumnWidth(80, Resizable = false)]
    [GUIColor("GetColorForRoomType")]
    public RoomType RoomType;

    #region Battle
    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public int Difficulty = 3;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public int TotalStartValue = 6;

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RoomType.Battle)]
    public bool PredetermineDeck;
    
    [FoldoutGroup("Room Data")]
    [ShowIf("@ShouldShowDeckBlueprint()")]
    public Deck Deck;

    #endregion

    #region Card Picking

    [FoldoutGroup("Room Data")]
    [ShowIf("roomType", RewardType.Cards)]
    public int amountOfOptions;
    
    #endregion

    private bool ShouldShowDeckBlueprint()
    {
        return RoomType is RoomType.Battle && PredetermineDeck;
    }

    private Color GetColorForRoomType()
    {
        Color color = Color.white;
        switch (RoomType)
        {
            case RoomType.Battle:
                //Muted red
                color = new Color(0.8f, 0.3f, 0.3f, 1.0f);
                break;
        }

        //Muted green
        //color = new Color(0.3f, 0.6f, 0.3f, 1.0f);

        //Muted blue
        //color = new Color(0.3f, 0.4f, 0.8f, 1.0f);

        //muted yellow
        //color = new Color (0.9f, 0.9f, 0.5f, 1.0f)  

        return color;
    }
}


public enum RoomType
{
    First,
    Battle,
    Boss,
}

public enum RewardType
{
    Nothing,
    Cards,
    Money,
}
