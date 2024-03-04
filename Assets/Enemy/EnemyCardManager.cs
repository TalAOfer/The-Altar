using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardManager : MonoBehaviour
{
    private BattleStateMachine _ctx;
    private FloorData _floorData;
    private GameObject _cardPrefab;
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

    [Button]
    public void ReorderPlaceholders(int amount)
    {
        int enemyCount = amount;
        if (enemyCount == 0) return;

        int formationIndex = Mathf.Clamp(enemyCount - 1, 0, _formations.Formations.Count - 1);
        EnemyFormation selectedFormation = _formations.Formations[formationIndex];

        int rowCount = selectedFormation.RowAmount;
        float rowSpacing = selectedFormation.RowSpacing;
        float columnSpacing = selectedFormation.ColumnSpacing;

        int enemiesPerRow = enemyCount / rowCount;
        int remainingEnemies = enemyCount % rowCount; // For the last row if it has fewer enemies

        for (int i = 0, currentEnemy = 0; i < rowCount; i++)
        {
            // Calculate the number of enemies in the current row
            int enemiesInThisRow = enemiesPerRow + (remainingEnemies > 0 && i == rowCount - 1 ? remainingEnemies : 0);
            // Calculate the starting X position to center the enemies in the current row
            float startXPosition = -(enemiesInThisRow - 1) * columnSpacing / 2;

            for (int j = 0; j < enemiesInThisRow; j++, currentEnemy++)
            {
                float xPosition = startXPosition + (j * columnSpacing);
                // Adjust Y position based on the row, -1 for back row, 1 for front row, and multiply by rowSpacing
                float yPosition = (rowCount <= 1) ? 0 : (i - (rowCount / 2.0f) + 0.5f) * rowSpacing;

                // Position the placeholder
                _enemyPlaceholders[currentEnemy].localPosition = new Vector3(xPosition, yPosition, 0);
                _enemyPlaceholders[currentEnemy].gameObject.SetActive(true);
            }
        }

        // Deactivate any unused placeholders
        for (int i = enemyCount; i < _enemyPlaceholders.Count; i++)
        {
            _enemyPlaceholders[i].gameObject.SetActive(false);
        }
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
