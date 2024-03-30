using DG.Tweening;
using Sirenix.OdinInspector;
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
    private PrefabRegistry _prefabs;
    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _events = Locator.Events;
        _prefabs = Locator.Prefabs;
        _chestStartTransform = transform.localPosition;
        Fall();
    }

    public void Fall()
    {
        _sr.sprite = closedSprite;
        transform.localPosition = _chestStartTransform;
        transform.DOLocalMoveY(0, 1).SetEase(ease).OnComplete(() =>
        {
            _events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);
            _sr.sprite = openSprite;
            Instantiate(_prefabs.BoosterPack);
        });

    }
}
