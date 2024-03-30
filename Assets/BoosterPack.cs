using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPack : MonoBehaviour
{
    [SerializeField] private int amountOfClicks;
    private int amountClicked;

    [SerializeField] private float xOffset;
    [SerializeField] private int amountOfCards;
    [SerializeField] private Vector2Int cardLevelMinMax;

    List<Card> cards = new();
    private SpriteRenderer sr;

    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private float moveYAmount;
    [SerializeField] TweenBlueprint floaty;
    [SerializeField] private LoopType _loopType;

    private MetaPoolInstance _playerPool;

    private Tweener _tweener;
    private Vector3 _startPos;
    private Vector3 _startScale;
    private Collider2D _coll;

    private void Awake()
    {
        _startPos = transform.localPosition;
        _startScale = transform.localScale;
        _tweener = GetComponent<Tweener>();
        _playerPool = Locator.RunData.playerPool;
        sr = GetComponent<SpriteRenderer>();
        _coll = GetComponent<Collider2D>();
        Initialize();
        Test();
    }

    public void Initialize()
    {
        for (int i = 0; i < amountOfCards; i++)
        {
            CardBlueprint cardBlueprint = _playerPool.GetRandomCardByPoints(cardLevelMinMax.x, cardLevelMinMax.y, null);
            Card card = CardSpawner.Instance.SpawnCard(cardBlueprint, transform.position, GameConstants.PLAYER_CARD_LAYER,
            CardInteractionType.Selection, null, null);
            card.transform.SetParent(transform, false);
            card.visualHandler.SetSortingOrder(i);
            card.interactionHandler.SetInteractability(false);
            cards.Add(card);
        }
    }

    [Button]
    public void Test()
    {
        transform.localPosition = _startPos;
        transform.localScale = _startScale;
        DOTween.Kill(transform);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(Vector3.one, duration).SetEase(ease));
        sequence.Insert(0, transform.DOMoveY(transform.position.y + moveYAmount, duration).SetEase(ease));
        sequence.OnComplete(() =>
        {
            transform.DOScale(Vector3.one * 1.1f, 0.5f)
                .SetLoops(-1, _loopType)
                .SetEase(Ease.InOutQuad);
        });


    }

    public void OnClick()
    {
        amountClicked++;

        if (amountClicked >= amountOfClicks)
        {
            Open();
            //bandaid for misclicking at the end again
            amountClicked = -10;
        }
    }

    public void Open()
    {
        DOTween.Kill(transform);

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(transform.DOMove(Vector3.zero, 0.25f));
        mySequence.AppendCallback(() =>
        {
            sr.enabled = false;
            _coll.enabled = false;
        });

        for (int i = 0; i < cards.Count; i++)
        {
            float finalXPos = (i - (cards.Count - 1) / 2.0f) * xOffset;
            mySequence.Join(cards[i].transform.DOLocalMoveX(finalXPos, 0.25f));
        }

        mySequence.OnComplete(() =>
        {
            foreach (var card in cards)
            {
                card.interactionHandler.SetInteractability(true);
            }
        });
    }
}
