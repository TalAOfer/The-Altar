using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu (menuName ="Game State")]
[System.Serializable]
public class CurrentGameState : ScriptableObject
{
    public GameState currentState;
}

public enum GameState
{
    Idle,
    BattleFormation,
    Battle,
    DrawCard,
    PickEnemySlot,
    SacrificeFormation,
    Sacrifice,
    SelectPlayerCard,
    ChooseEnemyCard,
    ChooseNewBlueprints,
    GameSetup,
    RoomSetup
}
