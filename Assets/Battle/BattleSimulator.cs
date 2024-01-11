using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSimulator : MonoBehaviour
{
    [SerializeField] private CardBlueprint attackedCardBlueprint;
    [SerializeField] private CardBlueprint attackingCardBlueprint;

    [SerializeField] private GameObject revealedCardPrefab;

    [SerializeField] private Transform mapMasterContainer;
    [SerializeField] private Transform topContainer;
    [SerializeField] private Transform botttomContainer;

    [SerializeField] private Vector3 outOfScreenBoundsPosition;

    [Button]
    public void DrawCards()
    {
    //    Card attackedCard = SpawnCard(attackedCardBlueprint, CardOwner.Enemy, 0, GameConstants.TOP_MAP_LAYER);
    //    attackedCard.transform.position = topContainer.position;
    //    attackedCard.interactionHandler.SetNewDefaultLocation(attackedCard.transform.position, attackedCard.transform.localScale, attackedCard.transform.eulerAngles);
    //    Card attackingCard = SpawnCard(attackingCardBlueprint, CardOwner.Player, 0, GameConstants.HAND_LAYER);
    //    attackingCard.transform.position = botttomContainer.position;
    //    attackingCard.interactionHandler.SetNewDefaultLocation(attackingCard.transform.position, attackingCard.transform.localScale, attackingCard.transform.eulerAngles);
    }
}
