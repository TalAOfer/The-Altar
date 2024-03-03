using System.Collections.Generic;
using UnityEngine;

public class EnemyCardManager : MonoBehaviour
{
    private BattleStateMachine _ctx;
    private FloorData _floorData;
    private GameObject _cardPrefab;
    private Codex Codex => _floorData.enemyCodex;
    public List<Card> ActiveEnemies { get; private set; } = new();

    public void Init(BattleStateMachine ctx)
    {
        _ctx = ctx;
        _floorData = Locator.FloorData;
        _cardPrefab = Locator.Prefabs.Card;

        ActiveEnemies.Clear();
    }

    public void AddEnemyToManager(Card card)
    {
        ActiveEnemies.Add(card);
    }

    public void RemoveEnemyFromManager(Card card)
    {
        ActiveEnemies.Remove(card);
    }

    private Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName, Transform parent)
    {
        GameObject cardGO = Instantiate(_cardPrefab, transform.position, Quaternion.identity, parent);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(Codex, cardBlueprint, sortingLayerName, CardInteractionType.Playable, _ctx.DataProvider);

        return card;
    }

    public Card SpawnEnemyInIndexByBlueprint(int containerIndex, CardBlueprint cardBlueprint)
    {
        Card card = SpawnCard(cardBlueprint, GameConstants.ENEMY_CARD_LAYER, transform);
        card.transform.localPosition = Vector3.zero;
        card.index = containerIndex;

        //StartCoroutine(room.grid[containerIndex].SetSlotState(MapSlotState.Enemy));

        return card;
    }
}
