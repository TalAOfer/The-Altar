using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    private Vector3 _chestStartTransform;
    private SpriteRenderer _sr;
    private EventRegistry _events;
    [SerializeField] private TreasureBlueprint TreasureBlueprint;
    public Treasure Treasure {  get; private set; }

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _events = Locator.Events;
        _chestStartTransform = transform.localPosition;
    }

    public void Initialize()
    {
        Treasure = new Treasure(TreasureBlueprint);
    }

    public IEnumerator Fall()
    {
        _sr.sprite = closedSprite;
        transform.localPosition = _chestStartTransform;
        yield return transform.DOLocalMoveY(0, 1).SetEase(ease).WaitForCompletion();

        _events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);
        _sr.sprite = openSprite;

        yield break;
    }
}

public class Treasure
{
    public List<TreasureItem> Items { get; private set; } = new();
    public Treasure(TreasureBlueprint blueprint)
    {
        foreach(var itemBlueprint in  blueprint.Items)
        {
            Items.Add(itemBlueprint.InstantiateItem());
        }
    }
}


[Serializable]
public class TreasureItem
{
    public TreasureItemType ItemType;
    public int Amount;
    public Vector2Int MinMax;
    public TreasureItem(TreasureItemType type, int amount, Vector2Int minMax)
    {
        ItemType = type;
        Amount = amount;
        MinMax = minMax;
    }
}

