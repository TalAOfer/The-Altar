using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Base_SpawnTreasure : BaseRoomState<RoomStateMachine>
{
    private readonly PrefabRegistry _prefabs;
    public Base_SpawnTreasure(RoomStateMachine ctx) : base(ctx)
    {
        _prefabs = Locator.Prefabs;
    }

    public override IEnumerator EnterState()
    {
        _ctx.InstantiatePrefab(_prefabs.TreasureChest, _prefabs.TreasureChest.transform.position, Quaternion.identity, _ctx.transform);
        yield return null;
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        card.visualHandler.EnableOutline(PaletteColor.white);
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        Debug.Log("happened");
        card.visualHandler.DisableOutline();
    }

    public override void HandlePlayerCardPointerClick(Card card, PointerEventData eventData)
    {
        base.HandlePlayerCardPointerClick(card, eventData);
    }
}
