using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private BlueprintPoolBlueprint EnemyMetaPoolInstance;
    [SerializeField] private BlueprintPoolBlueprint enemyPoolInstance;

    [SerializeField] private BlueprintPoolBlueprint playerMetaPoolInstance;
    [SerializeField] private BlueprintPoolBlueprint playerPoolInstance;
    [SerializeField] private int amount;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform enemyTransform;

    public void SpawnSpecialCards()
    {

        for (int i = 0; i < amount; i++)
        {
            
        }
    }

    public Card SpawnCard(CardBlueprint cardBlueprint, int index, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
        cardGO.transform.localScale = Vector3.one * GameConstants.MapScale;
        cardGO.name = cardBlueprint.name;

        Card card = cardGO.GetComponent<Card>();
        //card.Init(blueprintPoolInstance, cardBlueprint, cardOwner, index, sortingLayerName);

        return card;
    }
}
