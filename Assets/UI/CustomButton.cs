using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private int index;
    [SerializeField] private CustomGameEvent response;

    [SerializeField] private bool animate;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private Reaction _ClickReaction;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private Reaction _PointerDownReaction;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private Reaction _PointerEnterReaction;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private Reaction _PointerExitReaction;

    private SpriteRenderer sr;
    private Collider2D coll;
    private EventRegistry _events;
    private Tweener tweener;
    [SerializeField] private bool disableAfterOneClick = true;


    public void RaiseRoomClickEvent()
    {
        if (_events != null) 
        {
            _events.OnRoomButtonClicked.Raise(this, index);
        }
    }

    private void Awake()
    {
        _events = Locator.Events;
        sr = GetComponentInChildren<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        if (animate) tweener = GetComponentInChildren<Tweener>();
    }
    public void SetInteractability(bool enable)
    {
        coll.enabled = enable;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (disableAfterOneClick)
        {
            SetInteractability(false);
        }
        response.Invoke(null, null);
        React(_ClickReaction);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        React(_PointerEnterReaction);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        React(_PointerExitReaction);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        React(_PointerDownReaction);
    }

    private void React(Reaction reaction)
    {
        if (reaction.Animate)
            tweener.TriggerTween(reaction.Animation);

        if (reaction.ChangeSprite)
            sr.sprite = reaction.Sprite;

        if (reaction.PlaySound)
        {
            Tools.PlaySound(reaction.soundEventName, transform);
        }
    }
}



[Serializable]
public class Reaction
{
    public bool Animate;
    [ShowIf("Animate")]
    public TweenBlueprint Animation;

    public bool ChangeSprite;
    [ShowIf("ChangeSprite")]
    public Sprite Sprite;

    public bool PlaySound;
    [ShowIf("PlaySound")]
    public string soundEventName;
}
