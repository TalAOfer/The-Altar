using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public static CardSpawner Instance { get; private set; }

    private PrefabRegistry _prefabRegistry;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _prefabRegistry = Locator.Prefabs;
    }



    public Card SpawnCard(CardBlueprint cardBlueprint, Vector3 position, Transform parent, string sortingLayerName, CardInteractionType cardType, Codex codex = null, BattleRoomDataProvider dataProvider = null)
    {
        GameObject cardGO = Instantiate(_prefabRegistry.Card, position, Quaternion.identity, parent);
        Card card = cardGO.GetComponent<Card>();
        card.Init(codex, cardBlueprint, sortingLayerName, cardType, dataProvider);

        return card;
    }
}
