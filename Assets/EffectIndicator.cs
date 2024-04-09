using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectIndicator : MonoBehaviour
{
    private TextMeshProUGUI textToFade; // Assign this in the inspector

    private void Awake()
    {
        textToFade = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        float currentY = transform.localPosition.y;
        transform.DOLocalMoveY(currentY + 2, 1.5f).SetEase(Ease.InQuad);
        textToFade.DOFade(0, 1.25f).SetEase(Ease.InQuad);
    }
}
