using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;

    [FoldoutGroup("Player")]
    [SerializeField] private PlayerDeck playerDeck;
    [FoldoutGroup("Player")]
    [SerializeField] private HandManager handManager;
    [FoldoutGroup("Player")]
    [SerializeField] private PlayerManager playerManager;
    [FoldoutGroup("Player")]
    [SerializeField] private int cardStartingAmount;

    [FoldoutGroup("Enemy")]
    [SerializeField] private EnemyDeck enemyDeck;
    [FoldoutGroup("Enemy")]
    [SerializeField] private MapConfigs mapConfig;

    private void Awake()
    {
        events.SetGameState.Raise(this, GameState.Setup);
    }

    [Button]
    public void DrawMap()
    {
        StartCoroutine(SetupRoutine());
    }

    public IEnumerator SetupRoutine()
    {
        yield return StartCoroutine(DealPlayer());
        yield return StartCoroutine(DealEnemies());

        yield return StartCoroutine(OnObtainRoutine());

        //TODO: Activate enemy Meditate
        
        events.SetGameState.Raise(this, GameState.Idle);

    }


    private IEnumerator DealPlayer()
    {
        for (int i = 0; i < cardStartingAmount; i++)
        {
            events.DrawPlayerCardToHand.Raise(this, null);

            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator DealEnemies()
    {
        int randForConfig = Random.Range(0, 8);
        Vector2Int randomConfig = mapConfig.options[randForConfig];

        for (int i = 0; i < 2; i++)
        {
            bool firstCard = (i == 0);
            int firstCardConfig = randomConfig.x;
            int secondCardConfig = randomConfig.y;

            int slotIndex = firstCard ? firstCardConfig : secondCardConfig;

            events.DrawEnemyCardToMapIndex.Raise(this, slotIndex);

            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator OnObtainRoutine()
    {
        foreach (Card card in playerManager.activeCards)
        {
            yield return StartCoroutine(card.effects.ApplyOnObtainEffects());
        }
    }

}
