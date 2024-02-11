using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer leftWall;
    [SerializeField] private SpriteRenderer rightWall;
    [SerializeField] private SpriteRenderer wholeWall;
    [SerializeField] private GameObject buttons;
    private AllEvents events;

    private void Awake()
    {
        events = Tools.GetEvents();
    }

    [Button]
    public void CloseWalls()
    {
        float duration = 1.0f; // Duration of the animation
        float targetPosX = 0f; // Center x position

        Tools.PlaySound("Scene_Transition", transform);
        rightWall.DOFade(1, duration);
        rightWall.transform.DOMoveX(targetPosX, duration).SetEase(Ease.OutQuad);

        leftWall.DOFade(1, duration);
        leftWall.transform.DOMoveX(targetPosX, duration).SetEase(Ease.OutQuad).OnComplete(() =>

        {
            rightWall.gameObject.SetActive(false);
            leftWall.gameObject.SetActive(false);
            wholeWall.gameObject.SetActive(true);
            buttons.SetActive(true);
            events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);
        });
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
