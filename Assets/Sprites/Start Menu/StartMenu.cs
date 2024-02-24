using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Tweener backgroundTweener;
    [SerializeField] private TweenBlueprint scaleTween;
    [SerializeField] private SpriteRenderer maskSr;
    private void Awake()
    {
        Tools.PlaySound("Ambient", transform);
    }

    public void Zoom()
    {
        maskSr.DOFade(1, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Tools.PlaySound("Scene_Transition", transform);
            backgroundTweener.TriggerTween(scaleTween).OnComplete(() =>
            {
                Locator.Events.LoadScene.Raise(this, 1);
            });
        });
    }
}
