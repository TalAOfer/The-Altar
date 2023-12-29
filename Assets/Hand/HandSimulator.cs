using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSimulator : MonoBehaviour
{
    public CardBlueprint cardBlueprint;
    public HandManager handManager;
    [SerializeField] private Vector3 outOfScreenBoundsPosition;
    [SerializeField] private Transform mapMasterContainer;
    [SerializeField] private int amount;
    [SerializeField] private GameObject revealedCardPrefab;
    [SerializeField] private float cardScale;

    [Button]
    public void DrawCards()
    {
        for (int i = 0; i < amount; i++)
        {
            Card card = SpawnCard(cardBlueprint, CardOwner.Player, i, GameConstants.TOP_MAP_LAYER);
            handManager.AddCardToHand(card);
            card.transform.localScale = Vector3.one * cardScale;
        }
    }

    private Card SpawnCard(CardBlueprint cardBlueprint, CardOwner cardOwner, int index, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(revealedCardPrefab, outOfScreenBoundsPosition, Quaternion.identity, mapMasterContainer);
        cardGO.transform.localScale = Vector3.one;
        cardGO.name = cardBlueprint.name;

        Card card = cardGO.GetComponent<Card>();
        card.Init(cardBlueprint, cardOwner, index, sortingLayerName);

        return card;
    }
}
