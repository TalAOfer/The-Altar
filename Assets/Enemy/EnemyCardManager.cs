using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardManager : MonoBehaviour
{
    private BattleStateMachine _ctx;
    private FloorData _floorData;
    private GameObject _cardPrefab;
    private Vector3 _outsideScreenPosition = new(0, 30, 0);
    [SerializeField] CardData _cardData;
    private Codex Codex => _floorData.enemyCodex;
    [SerializeField] private EnemyFormationRegistry _formations;
    public List<Card> ActiveEnemies { get; private set; } = new();
    [SerializeField] private List<Transform> _enemyPlaceholders = new();

    public void Init(BattleStateMachine ctx)
    {
        _ctx = ctx;
        _floorData = Locator.FloorData;
        _cardPrefab = Locator.Prefabs.Card;

        ActiveEnemies.Clear();
    }
    #region Formation
    [Button]
    public void ReorderPlaceholders()
    {
        int enemyCount = ActiveEnemies.Count;
        if (enemyCount == 0) return;

        int formationIndex = Mathf.Clamp(enemyCount - 1, 0, _formations.Formations.Count - 1);
        EnemyFormation selectedFormation = _formations.Formations[formationIndex];

        int rowCount = selectedFormation.RowAmount;
        float rowSpacing = selectedFormation.RowSpacing;
        float columnSpacing = selectedFormation.ColumnSpacing;

        int enemiesPerRow = enemyCount / rowCount;
        int remainingEnemies = enemyCount % rowCount; // For the last row if it has fewer enemies

        int currentEnemyIndex = 0; // Track the current index of ActiveEnemies list

        for (int i = 0; i < rowCount; i++)
        {
            // Calculate the number of enemies in the current row
            int enemiesInThisRow = enemiesPerRow + (remainingEnemies > 0 && i == rowCount - 1 ? remainingEnemies : 0);
            // Calculate the starting X position to center the enemies in the current row
            float startXPosition = -(enemiesInThisRow - 1) * columnSpacing / 2;

            for (int j = 0; j < enemiesInThisRow; j++)
            {
                // Calculate the position for the current placeholder
                float xPosition = startXPosition + (j * columnSpacing);
                float yPosition = (rowCount <= 1) ? 0 : (i - (rowCount / 2.0f) + 0.5f) * rowSpacing;

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
    }

    public void ResetCardsToPlaceholders()
    {
        foreach (var card in ActiveEnemies)
        {
            StartCoroutine(card.movement.TransformCardUniformlyToPlaceholder(_cardData.ReorderSpeed, _cardData.ReorderCurve));
        }
    }

    #endregion

    public void AddEnemyToManager(Card card)
    {
        ActiveEnemies.Add(card);
        ReorderPlaceholders();
    }

    public void RemoveEnemyFromManager(Card card)
    {
        ActiveEnemies.Remove(card);
    }

    public Card SpawnCard(CardBlueprint cardBlueprint, Transform parent)
    {
        GameObject cardGO = Instantiate(_cardPrefab, _outsideScreenPosition, Quaternion.identity, parent);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(Codex, cardBlueprint, GameConstants.ENEMY_CARD_LAYER, CardInteractionType.Playable, _ctx.DataProvider);

        return card;
    }
}
