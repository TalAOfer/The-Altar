using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCardManager : MonoBehaviour
{
    private RoomStateMachine _sm;
    private Codex EnemyCodex => _sm.EnemyCodex;
    private BattleRoomDataProvider _dataProvider;

    private GameObject _cardPrefab;
    private Vector3 _outsideScreenPosition = new(0, 30, 0);
    [SerializeField] CardData _cardData;
    public List<Card> ActiveEnemies { get; private set; } = new();
    [SerializeField] private List<Transform> _enemyPlaceholders = new();
    [SerializeField] private Transform _container;
    [SerializeField] private Vector2 _enemySpacing = new(2.25f, 4f);

    public void Initialize(RoomStateMachine sm, BattleRoomDataProvider dataProvider)
    {
        _sm = sm;
        _cardPrefab = Locator.Prefabs.Card;
        _dataProvider = dataProvider;
        ActiveEnemies.Clear();
    }

    #region Formation
    [Button]
    public void ReorderPlaceholders(bool shouldMoveCards)
    {
        int enemyCount = ActiveEnemies.Count;
        if (enemyCount == 0) return;

        int rowCount = enemyCount > 3 ? 2 : 1;

        int enemiesPerRow = enemyCount / rowCount;
        int remainingEnemies = enemyCount % rowCount; // For the last row if it has fewer enemies

        int currentEnemyIndex = 0; // Track the current index of ActiveEnemies list

        for (int i = 0; i < rowCount; i++)
        {
            // Calculate the number of enemies in the current row
            int enemiesInThisRow = enemiesPerRow + (remainingEnemies > 0 && i == rowCount - 1 ? remainingEnemies : 0);
            // Calculate the starting X position to center the enemies in the current row
            float startXPosition = -(enemiesInThisRow - 1) * _enemySpacing.x / 2;

            for (int j = 0; j < enemiesInThisRow; j++)
            {
                // Calculate the position for the current placeholder
                float xPosition = startXPosition + (j * _enemySpacing.x);
                float yPosition = (rowCount <= 1) ? 0 : (i - (rowCount / 2.0f) + 0.5f) * -_enemySpacing.y;

                // Position the placeholder
                _enemyPlaceholders[currentEnemyIndex].localPosition = new Vector3(xPosition, yPosition, 0);
                _enemyPlaceholders[currentEnemyIndex].gameObject.SetActive(true);

                // Assign placeholder to the corresponding card in ActiveEnemies
                if (currentEnemyIndex < ActiveEnemies.Count)
                {
                    // Access the card and assign its movement placeholder
                    Card card = ActiveEnemies[currentEnemyIndex];
                    card.movement.placeHolder = _enemyPlaceholders[currentEnemyIndex];
                }

                currentEnemyIndex++; // Move to the next card in ActiveEnemies list
            }
        }

        // Deactivate any unused placeholders
        for (int i = enemyCount; i < _enemyPlaceholders.Count; i++)
        {
            _enemyPlaceholders[i].gameObject.SetActive(false);
        }

        if (shouldMoveCards) StartCoroutine(ResetCardsToPlaceholders());
    }

    public IEnumerator ResetCardsToPlaceholders()
    {
        foreach (var card in ActiveEnemies)
        {
            if (card.cardState is CardState.Default)
            {
                yield return (card.movement.TransformCardUniformlyToPlaceholder(_cardData.ReorderSpeed, _cardData.ReorderCurve));
            }
        }

        yield break;
    }

    #endregion

    public void AddEnemyToManager(Card card)
    {
        ActiveEnemies.Add(card);
        ReorderPlaceholders(false);
    }

    public void RemoveEnemyFromManager(Card card)
    {
        ActiveEnemies.Remove(card);
    }

    public Card SpawnCard(CardBlueprint cardBlueprint)
    {
        GameObject cardGO = Instantiate(_cardPrefab, _outsideScreenPosition, Quaternion.identity, _container);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(EnemyCodex, cardBlueprint, GameConstants.ENEMY_CARD_LAYER, CardInteractionType.Playable, _dataProvider);
        AddEnemyToManager(card);
        card.ChangeCardState(CardState.Draw);

        return card;
    }

    public IEnumerator SpawnEnemiesByArchetype(CardArchetype archetype, int amount)
    {
        List<CardBlueprint> cardBlueprintsToSpawn = new();
        CardBlueprint cardBlueprint = EnemyCodex.GetCardOverride(archetype);
        for (int i = 0; i < amount; i++)
        {
            cardBlueprintsToSpawn.Add(cardBlueprint);
        }

        yield return SpawnEnemies(cardBlueprintsToSpawn);
    }

    public IEnumerator SpawnEnemies(CardBlueprint enemyBlueprint)
    {
        Card cardSpawned = SpawnCard(enemyBlueprint);
        yield return ResetCardsToPlaceholders();
        yield return Tools.GetWait(1);
        cardSpawned.transform.position = cardSpawned.movement.placeHolder.position;
        cardSpawned.ChangeCardState(CardState.Default);
    }

    public IEnumerator SpawnEnemies(List<CardBlueprint> enemyBlueprints)
    {
        List<Card> cardsSpawned = new();

        for (int i = 0; i < enemyBlueprints.Count; i++)
        {
            CardBlueprint cardBlueprint = enemyBlueprints[i];
            Card cardSpawned = SpawnCard(cardBlueprint);
            cardsSpawned.Add(cardSpawned);
        }

        yield return ResetCardsToPlaceholders();
        yield return new WaitForSeconds(0.50f);

        foreach (Card cardSpawned in cardsSpawned)
        {
            cardSpawned.transform.position = cardSpawned.movement.placeHolder.position;
            cardSpawned.ChangeCardState(CardState.Default);
            yield return Tools.GetWait(0.25f);
        }
    }


}
