using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private MapConfigs mapConfig;

    [SerializeField] private GameObject upsideDownCardPrefab;
    [SerializeField] private GameObject revealedCardPrefab;

    [SerializeField] private Transform[] handContainers;

    [SerializeField] private Transform mapMasterContainer;
    [SerializeField] private Transform[] mapCardContainers;

    [SerializeField] private Vector3 outOfScreenBoundsPosition;



    private Card SpawnCard(CardBlueprint cardBlueprint, CardStates cardState)
    {
        GameObject prefabToSpawn = cardState == CardStates.UpsideDown ? upsideDownCardPrefab : revealedCardPrefab;
        GameObject cardGO = Instantiate(prefabToSpawn, outOfScreenBoundsPosition, Quaternion.identity, mapMasterContainer);
        cardGO.transform.localScale = Vector3.one;

        Card card = cardGO.GetComponent<Card>();
        card.Init(cardBlueprint);

        return card;
    }

    [Button]
    public void DrawMap()
    {
        StartCoroutine(DealMap());
    }

    private IEnumerator DealMap()
    {
        for (int i = 0; i < 9; i++)
        {
            Card card = SpawnCard(deck.cards[0], CardStates.UpsideDown);
            card.transform.position = mapCardContainers[i].position;
            yield return new WaitForSeconds(0.25f);
        }

        StartCoroutine(DealPlayer());
    }

    private IEnumerator DealPlayer()
    {
        for (int i = 0; i < 3; i++)
        {
            Card card = SpawnCard(deck.cards[0], CardStates.Revealed);
            card.transform.position = handContainers[i].position;
            yield return new WaitForSeconds(0.25f);
        }
        StartCoroutine(DealEnemies());
    }

    private IEnumerator DealEnemies()
    {
        int randForConfig = Random.Range(0, 8);
        Vector2Int randomConfig = mapConfig.options[randForConfig];

        for (int i = 0; i < 2; i++)
        {
            int containerIndex = (i == 0) ? randomConfig.x : randomConfig.y;
            Card card = SpawnCard(deck.cards[0], CardStates.Revealed);
            card.transform.position = mapCardContainers[containerIndex].position;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
