using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SymbolColumnsHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> upColumns;
    [SerializeField] private List<Transform> downColumns;
    [SerializeField] private float upSpeed;
    [SerializeField] private float downSpeed;

    private void Start()
    {
        foreach (Transform column in upColumns)
        {
            TweenColumnUp(column);
        }
        
        foreach (Transform column in downColumns)
        {
            TweenColumnDown(column);
        }
    }

    private void OnDisable()
    {
        foreach (Transform column in upColumns)
        {
            DOTween.Kill(column);
        }

        foreach (Transform column in downColumns)
        {
            DOTween.Kill(column);
        }
    }

    private void TweenColumnUp(Transform column)
    {
        column.DOMoveY(12.5f, upSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
        {
            column.transform.position = new Vector3(column.transform.position.x, -17.5f, 0);
            TweenColumnUp(column);
        });
    }

    private void TweenColumnDown(Transform column)
    {
        column.DOMoveY(-12.5f, downSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
        {
            column.transform.position = new Vector3(column.transform.position.x, 17.5f, 0);
            TweenColumnDown(column);
        });
    }
}
