using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseManager : MonoBehaviour
{
    [SerializeField] private RectTransform leftWall;
    [SerializeField] private RectTransform rightWall;
    [SerializeField] private RectTransform wholeWall;
    [SerializeField] private GameObject buttons;
    [SerializeField] private TweenBlueprint blueprint;
    private AllEvents events;

    private void Awake()
    {
        events = Tools.GetEvents();
    }


    private void OnEnable()
    {
        CloseWalls();
    }

    [Button]
    public void CloseWalls()
    {
        float duration = 1.0f; // Duration of the animation
        float targetPosX = 0f; // Center x position

        Tools.PlaySound("Scene_Transition", transform);
        rightWall.DOAnchorPosX(targetPosX, duration).SetEase(Ease.OutQuad);
        leftWall.DOAnchorPosX(targetPosX, duration).SetEase(Ease.OutQuad).OnComplete(() =>

        {
            rightWall.gameObject.SetActive(false);
            leftWall.gameObject.SetActive(false);
            wholeWall.gameObject.SetActive(true);
            wholeWall.DOShakePosition(blueprint.shakeDuration, blueprint.shakeStrength, blueprint.shakeVibrato, blueprint.shakeRandomness, false, true)
                .SetEase(blueprint.ease);
            buttons.SetActive(true);
        });
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
