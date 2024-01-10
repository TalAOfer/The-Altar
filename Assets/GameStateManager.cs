using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public CurrentGameState gameState;
    [SerializeField] private AllEvents events;

    [SerializeField] private HandManager hand;

    public void SetGameState(Component sender, object data)
    {
        GameState newGameState = (GameState)data;
        if (gameState.currentState == newGameState)
        {
            Debug.Log(sender + " tried to change game state to same state which is: " + gameState.currentState);
        }
        else
        {
            gameState.currentState = newGameState;
            OnGameStateChanged();
        }
    }

    private void OnGameStateChanged()
    {
        switch (gameState.currentState)
        {
            case GameState.Idle:
                break;
            case GameState.BattleFormation:
                break;
            case GameState.Battle:
                break;
            case GameState.DrawCard:
                break;
            case GameState.PickEnemySlot:
                break;
            case GameState.SacrificeFormation:
                break;
            case GameState.Sacrifice:
                break;
            case GameState.SelectPlayerCard:
                events.ToggleCurtain.Raise(this, true);
                hand.ChangeHandState(HandState.Select);
                break;
            case GameState.ChooseEnemyCard:
                break;
            case GameState.Setup:
                break;
        }

    }
}
