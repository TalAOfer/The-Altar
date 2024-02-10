using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBattleManager : BattleManager
{
    [SerializeField] private GameObject shapeshiftText;
    [SerializeField] private GameObject attackText;
    [SerializeField] private TutorialBattleRoom room;
    private bool isFrozen;

    public void UnfreezeBattle()
    {
        isFrozen = false;
    }

    public override IEnumerator AnimateBackoff()
    {
        room.FadeCurtain(false);
        playerManager.Hand.AddCardToHand(playerCard);
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
            Vector3 textPos = playerCard.transform.position;
            textPos.x += 6f;
            shapeshiftText.transform.position = textPos;
            shapeshiftText.SetActive(true);
        }

        yield return base.DeathRoutine();

        if (isFirstFight)
        {
            isFrozen = true;
            room.SetRaycastBlock(true);
            room.SetInteractability(true);

            while (isFrozen)
            {
                yield return null;
            }

            room.SetRaycastBlock(false);
            room.SetInteractability(false);
            shapeshiftText.SetActive(false);
        }
    }

    protected override IEnumerator BattleRoutine()
    {
        attackText.SetActive(false);
        yield return StartCoroutine(base.BattleRoutine());

        for (int i = 0; i < roomData.PlayerManager.ActiveCards.Count; i++)
        {
            Card currentCard = roomData.PlayerManager.ActiveCards[i];
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
