using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool canDraw;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private AllEvents events;

    private void Awake()
    {
        sr.color = defaultColor;
    }

    public void EnableDraw()
    {
        canDraw = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canDraw) return;
        gameManager.SpawnPlayerCard();

        sr.color = defaultColor;
        canDraw = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canDraw) return;
        sr.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!canDraw) return;
        sr.color = defaultColor;
    }



}