using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public CurrentGameState gameState;
    [SerializeField] private EventRegistry events;

    private void OnDisable()
    {
        Pooler.ClearPools();
    }
    public void SetGameState(Component sender, object data)
    {
        GameState newGameState = (GameState)data;
        if (gameState.currentState == newGameState)
        {
            //Debug.Log(sender + " tried to change game state to same state which is: " + gameState.currentState);
        }
        else
        {
            gameState.currentState = newGameState;
            OnGameStateChanged(sender);
        }
    }

    private void OnGameStateChanged(Component sender)
    {
        switch (gameState.currentState)
        {
            case GameState.Idle:
                //events.ToggleCurtain.Raise(this, false);
                //handManager.ChangeHandState(HandState.Idle);
                //selectManager.selectButton.gameObject.SetActive(false);
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
                break;
            case GameState.ChooseEnemyCard:
                break;
            case GameState.RoomSetup:
                break;
            case GameState.GameSetup:
                break;
        }
    }
}
