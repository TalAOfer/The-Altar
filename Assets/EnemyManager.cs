using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int xAmount = 0;
    public List<Card> activeEnemies;
    [SerializeField] private MapGridArranger grid;
    public void GetXAmount(Component sender, object data)
    {
        ActiveEffect askerEffect = (ActiveEffect)sender;

        StartCoroutine(askerEffect.HandleResponse(this, xAmount));
    }

    public void AddEnemyToManager(Card card)
    {
        activeEnemies.Add(card);
    }

    public void OnMapCardClicked(Component sender, object data)
    {
        int clickedCardIndex = (int)data;

        for (int i = 0; i < grid.MapSlots.Count; i++)
        {
            MapSlot slot = grid.MapSlots[i];
            if (i == clickedCardIndex) StartCoroutine(slot.SetSlotState(MapSlotState.Occupied));

            else if (slot.slotState == MapSlotState.Choosable)
            {
                StartCoroutine(slot.SetSlotState(MapSlotState.Idle));
            }
        }
    }

    public void HandleEnemyDeath(Card enemyCard)
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

    public void HightlightNeighboringSlots(Component sender, object data)
    {
        int beatenCardIndex = (int)data;
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


}
