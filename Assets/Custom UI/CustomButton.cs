using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CustomGameEvent response;
    [FoldoutGroup("Components")]
    [SerializeField] private Collider2D coll;
    [FoldoutGroup("Components")]
    [SerializeField] private SpriteRenderer sr;
    [FoldoutGroup("Components")]
    [SerializeField] private Tweener tweener;

    private void Awake()
    {
        SetInteractability(true);
    }

    public void SetInteractability(bool enable)
    {
        coll.enabled = enable;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //tweener.TriggerShake();
        response.Invoke(null, null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //tweener.TriggerJiggle();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //tweener.TriggerBounce();
    }
}
