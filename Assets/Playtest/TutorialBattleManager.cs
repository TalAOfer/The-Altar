using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBattleManager : BattleManager
{
    [SerializeField] private GameObject shapeshiftText;
    [SerializeField] private GameObject attackText;
    [SerializeField] private TutorialBattleRoom room;
    
    public override IEnumerator AnimateBackoff()
    {
        room.FadeCurtain(false);
        playerManager.hand.AddCardToHand(playerCard);
        playerCard.visualHandler.SetSortingOrder(playerCard.index);
        yield return StartCoroutine(playerCard.movement.TransformCardUniformlyToHoveredPlaceholder(cardData.backOffSpeed, cardData.backoffCurve));
        bool isFirstFight = playerCard.visualHandler.GetSortingLayer() == "Top";
        string afterBattleLayer = isFirstFight ? "Top" : GameConstants.PLAYER_CARD_LAYER;
        playerCard.visualHandler.SetSortingLayer(afterBattleLayer);
    }

    protected override IEnumerator DeathRoutine()
    {
        bool isFirstFight = playerCard.visualHandler.GetSortingLayer() == "Top";
        if (isFirstFight)
        {
            shapeshiftText.SetActive(true);
            Vector3 textPos = playerCard.transform.position;
            textPos.x += 6f;
            shapeshiftText.transform.position = textPos;
        }

        yield return base.DeathRoutine();
        
        if (isFirstFight)
        {
            yield return new WaitForSeconds(2f);
            shapeshiftText.SetActive(false);
        }
    }

    protected override IEnumerator BattleRoutine()
    {
        attackText.SetActive(false);
        yield return StartCoroutine(base.BattleRoutine());
        
        for (int i = 0; i < roomData.PlayerManager.activeCards.Count; i++)
        {
            Card currentCard = roomData.PlayerManager.activeCards[i];
            if (i != 2)
            {
                currentCard.interactionHandler.SetInteractability(true);
            }

            else
            {
                currentCard.visualHandler.SetSortingLayer(GameConstants.PLAYER_CARD_LAYER);
            }
        }

        foreach (Card card in roomData.EnemyManager.activeEnemies)
        {
            card.interactionHandler.SetInteractability(true);
        }
    }
}
