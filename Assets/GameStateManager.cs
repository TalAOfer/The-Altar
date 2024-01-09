using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public CurrentGameState gameState;
    [SerializeField] private AllEvents events;

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
        }
    }
}
