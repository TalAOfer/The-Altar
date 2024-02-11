using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessSpriteScroller : MonoBehaviour
{
    [SerializeField] private float startYPos;
    [SerializeField] private float endYPos;
    [SerializeField] private List<Transform> downColumns;
    [SerializeField] private float speed;

    private void Start()
    {
        foreach (Transform column in downColumns)
        {
            TweenColumnDown(column);
        }
    }

    private void OnDisable()
    {
        foreach (Transform column in downColumns)
        {
            DOTween.Kill(column);
        }
    }


    private void TweenColumnDown(Transform column)
    {
        column.DOLocalMoveY(endYPos, speed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
        {
            column.transform.localPosition = new Vector3(column.transform.position.x, startYPos, 0);
            TweenColumnDown(column);
        });
    }

}
