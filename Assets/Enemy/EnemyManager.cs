using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{


    [SerializeField] private MapGridArranger grid;
    public List<Card> activeEnemies;

    public void AddEnemyToManager(Card card)
    {
        activeEnemies.Add(card);
    }


    #region Event Getters

    public void GetRevealedEnemyCards(Component sender, object data)
    {
        ActiveEffect askerEffect = (ActiveEffect)sender;

        StartCoroutine(askerEffect.HandleResponse(this, activeEnemies));
    }

    #endregion
}
