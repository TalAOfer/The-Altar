using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<Card> activeEnemies;
    public AllEvents events;
    public EnemyDeck enemyDeck;
    public int Difficulty; 
    public void AddEnemyToManager(Card card)
    {
        activeEnemies.Add(card);
    }

    public void RemoveEnemyFromManager(Card card)
    {
        activeEnemies.Remove(card);
        if (activeEnemies.Count == 0)
        {

        }
    }

    [Button] 
    public void DealEnemies()
    {
        List<int> enemyConfig = Tools.DivideRandomly(Difficulty, 1, Difficulty);
        List<int> slots = Tools.GetXUniqueRandoms(enemyConfig.Count, 0, 8);

        for (int i = 0; i < enemyConfig.Count; i++)
        {
            int slotIndex = slots[i];
            int strength = enemyConfig[i];

            Card enemy = enemyDeck.SpawnEnemyInIndexByStrength(slotIndex, strength);

            activeEnemies.Add(enemy);
        }
    }

    [Button]
    public void DestroyAllEnemies()
    {
        List<Card> enemiesToDestroy = new(activeEnemies);
        foreach(Card card in enemiesToDestroy)
        {
            Destroy(card.gameObject);
        }

        activeEnemies.Clear();
    }

}
