using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Tweener backgroundTweener;
    [SerializeField] private SpriteRenderer maskSr;
    [SerializeField] private RunData runData;
    private void Awake()
    {
        Tools.PlaySound("Ambient", transform);
    }

    public void StartRun()
    {
        runData.Initialize();

        maskSr.DOFade(1, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Tools.PlaySound("Scene_Transition", transform);
            backgroundTweener.transform.DOScale(8, 1).OnComplete(() =>
            {
                Locator.Events.LoadScene.Raise(this, 1);
            });
        });
    }
}
