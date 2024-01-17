using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyCardSpawner spawner;
    public List<Card> activeEnemies;

    public void AddEnemyToManager(Card card)
    {
        activeEnemies.Add(card);
    }


    #region Event Getters

    public void GetRevealedEnemyCards(Component sender, object data)
    {
        SelectEffect askerEffect = (SelectEffect)sender;

        StartCoroutine(askerEffect.HandleResponse(this, activeEnemies));
    }

    #endregion
}
