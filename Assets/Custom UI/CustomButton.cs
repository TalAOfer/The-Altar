using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CustomGameEvent response;
    [FoldoutGroup("Components")]
    [SerializeField] private Collider2D coll;
    [FoldoutGroup("Components")]
    [SerializeField] private SpriteRenderer sr;

    [FoldoutGroup("UI Colors")]
    [SerializeField] private Color grey;
    [FoldoutGroup("UI Colors")]
    [SerializeField] private Color darkerWhite;
    [FoldoutGroup("UI Colors")]
    [SerializeField] private Color white;

    private void Awake()
    {
        SetInteractability(true);
    }

    public void SetInteractability(bool enable)
    {
        coll.enabled = enable;
        sr.color = enable ? darkerWhite : grey;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        response.Invoke(null, null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sr.color = darkerWhite;
    }
}
