using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPack : MonoBehaviour
{
    [SerializeField] private int amountOfClicks;

    public SpriteRenderer SR {  get; private set; }

    [SerializeField] private float duration;
    [SerializeField] private Ease ease;
    [SerializeField] private float moveYAmount;
    [SerializeField] TweenBlueprint floaty;
    [SerializeField] private LoopType _loopType;

    private Collider2D _coll;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    public IEnumerator AnimateOpening()
    {
        Sequence Enlarging = DOTween.Sequence();

        Enlarging.Append(transform.DOScale(Vector3.one, duration).SetEase(ease));
        Enlarging.Insert(0, transform.DOMoveY(transform.position.y + moveYAmount, duration).SetEase(ease));
        yield return Enlarging.WaitForCompletion();
   
    }
}
