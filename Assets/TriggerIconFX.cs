using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerIconFX : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float duration;
    [SerializeField] private Ease fadeEase;
    [SerializeField] private float yOffset;
    [SerializeField] private Ease offsetEase;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();    
    }

    private void OnEnable()
    {
        Color temp = sr.color;
        temp.a = 1f;
        sr.color = temp;
        sr.DOFade(0, duration).SetEase(fadeEase);
        float finalYPos = transform.position.y + yOffset;
        transform.DOMoveY(finalYPos, duration).SetEase(offsetEase);
    }

    [Button]
    public void StartFade()
    {
        Color temp = sr.color;
        temp.a = 1f;
        sr.color = temp;
        sr.DOFade(0, duration).SetEase(fadeEase);
        float finalYPos = transform.position.y + yOffset;
        transform.DOMoveY(finalYPos, duration).SetEase(offsetEase);
    }
}
