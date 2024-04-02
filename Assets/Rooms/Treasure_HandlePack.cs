
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Treasure_HandlePack : BaseRoomState
{
    Treasure Treasure => _ctx.currentTreasureChest.Treasure;
    TreasureItem Pack;

    MetaPoolInstance PlayerPool => RunData.playerPool;

    private float xOffset = 2.5f;

    private readonly List<Card> cards = new();
    private BoosterPack _pack;

    public Treasure_HandlePack(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        Pack = Treasure.Items[0];
        Treasure.Items.RemoveAt(0);
        GameObject packGO = _sm.InstantiatePrefab(Prefabs.BoosterPack, _ctx.currentTreasureChest.transform.position, Quaternion.identity, _ctx.currentTreasureChest.transform);
        _pack = packGO.GetComponent<BoosterPack>();
        yield return _pack.AnimateOpening();

        yield return SpawnCards();
    }

    private IEnumerator SpawnCards()
    {
        for (int i = 0; i < Pack.Amount; i++)
        {
            Debug.Log("Amount of cards: " + Pack.Amount);
            Debug.Log("Min: " + Pack.MinMax.x + "Max: " + Pack.MinMax.y);

            CardBlueprint cardBlueprint = PlayerPool.GetRandomCardByPoints(Pack.MinMax.x, Pack.MinMax.y, null);
            Card card = CardSpawner.Instance.SpawnCard(cardBlueprint, _pack.transform.position, _sm.transform, GameConstants.PLAYER_CARD_LAYER,
            CardInteractionType.Selection, null, null);
            card.transform.SetParent(_ctx.currentTreasureChest.transform, false);
            card.visualHandler.SetSortingOrder(i);
            card.interactionHandler.SetInteractability(false);
            cards.Add(card);
            card.gameObject.SetActive(false);
        }

        Sequence Opening = DOTween.Sequence();

        Opening.Append(_pack.transform.DOShakeRotation(1f, new Vector3(0, 0, 0.75f), 8, 1, true));
        Opening.AppendCallback(() =>
        {
            _pack.SR.enabled = false;
            foreach (Card card in cards)
            {
                card.gameObject.SetActive(true);
            }
        });

        for (int i = 0; i < cards.Count; i++)
        {
            float finalXPos = (i - (cards.Count - 1) / 2.0f) * xOffset;
            Opening.Join(cards[i].transform.DOLocalMoveX(finalXPos, 0.25f));
        }

        yield return Opening.WaitForCompletion();

        foreach (var card in cards)
        {
            card.interactionHandler.SetInteractability(true);
        }

        yield break;
    }

    public override void HandlePlayerCardPointerClick(Card clickedCard, PointerEventData eventData)
    {
        Events.HideTooltip.Raise();

        foreach (Card card in cards)
        {
            if (card == clickedCard)
            {
                RunData.playerCodex.OverrideCard(card.Mask);
            }
            
            else
            {
                RunData.playerPool.ReturnBlueprintToPool(card.Mask);
            }

            card.StartCoroutine(card.DestroySelf());
        }

        SwitchTo(States.OpenTreasure());
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        card.visualHandler.EnableOutline(PaletteColor.white);
        Events.ShowTooltip.Raise(_sm, card);
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        card.visualHandler.DisableOutline();
        Events.HideTooltip.Raise(_sm, card);
    }

}
