using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CustomGameEvent response;

    [SerializeField] private bool animate;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private Reaction clickReaction;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private Reaction hoverReaction;
    [ShowIf("animate")]
    [FoldoutGroup("Animations")]
    [SerializeField] private Reaction hoverOutReaction;

    private SpriteRenderer sr;
    private Collider2D coll;
    private Tweener tweener;


    private void Awake()
    {
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
        React(clickReaction);
        response.Invoke(null, null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        React(hoverReaction);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        React(hoverOutReaction);
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
