using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CustomGameEvent response;
   
    [SerializeField] private bool animate;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private TweenBlueprint clickReaction;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private TweenBlueprint hoverReaction;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private TweenBlueprint hoverOutReaction;
   
    private Collider2D coll;
    private Tweener tweener;
   

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        if (animate) tweener = GetComponentInChildren<Tweener>();
    }
    public void SetInteractability(bool enable)
    {
        coll.enabled = enable;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (animate) 
            tweener.TriggerTween(clickReaction);

        Tools.PlaySound("Click_Confirm", transform);
        response.Invoke(null, null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animate) 
            tweener.TriggerTween(hoverReaction);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animate)
            tweener.TriggerTween(hoverOutReaction);
    }
}
