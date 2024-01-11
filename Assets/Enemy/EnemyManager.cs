using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private MapGridArranger grid;
    public List<Card> activeEnemies;
    private int xAmount = 0;

    public void AddEnemyToManager(Card card)
    {
        activeEnemies.Add(card);
    }

    public void OnGameStateChange(Component sender, object data)
    {
        GameState newGameState = (GameState)data;
        switch (newGameState)
        {
            case GameState.Idle:

                break;
            case GameState.BattleFormation:
                break;
            case GameState.Battle:
                break;
            case GameState.SacrificeFormation:
                break;
            case GameState.Sacrifice:
                break;
            case GameState.SelectPlayerCard:
                break;
            case GameState.ChooseEnemyCard:
                break;
            case GameState.Setup:
                break;
        }
    }

    public void RestartSlotBlinking(Component sender, object data)
    {
        int clickedSlotIndex = (int)data;

        for (int i = 0; i < grid.MapSlots.Count; i++)
        {
            MapSlot slot = grid.MapSlots[i];
            if (i == clickedSlotIndex) StartCoroutine(slot.SetSlotState(MapSlotState.Occupied));

            else if (slot.slotState == MapSlotState.Choosable)
            {
                StartCoroutine(slot.SetSlotState(MapSlotState.Idle));
            }
        }
    }

    public void MarkAndDestroyDeadEnemy(Card enemyCard)
    {
        //Draw X
        int slotIndex = enemyCard.index;
        MapSlot slotOfDeadEnemy = grid.MapSlots[slotIndex];
        StartCoroutine(slotOfDeadEnemy.SetSlotState(MapSlotState.Done));
        xAmount += 1;

        //Remove and destroy
        activeEnemies.Remove(enemyCard);
        Destroy(enemyCard);
    }

    public void HightlightNeighboringSlots(int beatenCardIndex)
    {
        List<int> neighbors = grid.GetNeighbors(beatenCardIndex);

        foreach (int neighborIndex in neighbors)
        {
            MapSlot slot = grid.MapSlots[neighborIndex];
            if (slot.slotState == MapSlotState.Idle)
            {
                StartCoroutine(slot.SetSlotState(MapSlotState.Choosable));
            }
        }
    }

    #region Event Getters
    public void GetXAmount(Component sender, object data)
    {
        ActiveEffect askerEffect = (ActiveEffect)sender;

        StartCoroutine(askerEffect.HandleResponse(this, xAmount));
    }

    public void GetRevealedEnemyCards(Component sender, object data)
    {
        ActiveEffect askerEffect = (ActiveEffect)sender;

        StartCoroutine(askerEffect.HandleResponse(this, activeEnemies));
    }

    #endregion
}
