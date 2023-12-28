using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualHandler : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Animator anim;

    public SpriteFolder sprites;

    public SpriteRenderer cardSr;

    public SpriteRenderer iconSr;
    public SpriteRenderer numberSr;
    public SpriteRenderer symbolSr;

    [SerializeField] private float fadeLerpTime;

    public void Init(CardBlueprint blueprint, string startingSortingLayer)
    {
        symbolSr.sprite = GetSymbol(blueprint.symbol);
        iconSr.sprite = blueprint.cardSprite;
        numberSr.sprite = GetNumberSprite(card.points);
        SetSortingLayer(startingSortingLayer);

        if (card.cardState != CardState.Reward)
        {
            SetSpritesColor(blueprint.cardColor);
        }

        if (card.cardState == CardState.Reward) anim.Play("Card_UpsideDown");
        else anim.Play("Card_Idle");

    }

    public void UpdateNumberSprite()
    {
        numberSr.sprite = GetNumberSprite(card.points);
    }

    public void SetNewCardVisual(CardBlueprint blueprint)
    {
        symbolSr.sprite = GetSymbol(blueprint.symbol);
        iconSr.sprite = blueprint.cardSprite;
        numberSr.sprite = GetNumberSprite(card.points);
        SetSpritesColor(card.cardColor);
    }

    public void Reveal()
    {
        anim.Play("Card_Reveal");
    }

    public void SetCardBGColor(Color color)
    {
        cardSr.color = color;
    }

    public void SetSpritesColor(CardColor newColor)
    {
        card.cardColor = newColor;
        Color color = card.cardColor == CardColor.Black ? Color.black : Color.red;
        numberSr.color = color;
        symbolSr.color = color;
        iconSr.color = color;
    }

    public void SetSortingOrder(int index)
    {
        int calcIndex = index * 4;
        cardSr.sortingOrder = calcIndex;
        iconSr.sortingOrder = calcIndex + 1;
        numberSr.sortingOrder = calcIndex + 2;
        symbolSr.sortingOrder = calcIndex + 3;
    }

    public void SetSortingLayer(string sortingLayerName)
    {
        cardSr.sortingLayerName = sortingLayerName;
        numberSr.sortingLayerName = sortingLayerName;
        symbolSr.sortingLayerName = sortingLayerName;
        iconSr.sortingLayerName = sortingLayerName;
    }

    private Sprite GetNumberSprite(int currentPoints)
    {
        return sprites.numbers[currentPoints];
    }

    private Sprite GetSymbol(Symbol currentSymbol)
    {
        Sprite sprite = null;
        switch (currentSymbol)
        {
            case Symbol.Hearts:
                sprite = sprites.hearts;
                break;
            case Symbol.Clubs:
                sprite = sprites.clubs;
                break;
            case Symbol.Diamonds:
                sprite = sprites.diamonds;
                break;
            case Symbol.Spades:
                sprite = sprites.spades;
                break;
        }

        return sprite;
    }

    public void OnReveal()
    {
        anim.Play("Card_Idle");
        ShowSprites();
    }

    public void ShowSprites()
    {
        StartColorLerp(symbolSr, fadeLerpTime, false);
        StartColorLerp(numberSr, fadeLerpTime, false);
        StartColorLerp(iconSr, fadeLerpTime, false);
    }


    public void StartColorLerp(SpriteRenderer spriteRenderer, float duration, bool toTransparent)
    {
        StartCoroutine(LerpColorCoroutine(spriteRenderer, duration, toTransparent));
    }

    private IEnumerator LerpColorCoroutine(SpriteRenderer spriteRenderer, float duration, bool toTransparent)
    {
        if (spriteRenderer != null)
        {
            float elapsed = 0;
            Color startColor = spriteRenderer.color;
            Color endColor = toTransparent ?
                             new Color(startColor.r, startColor.g, startColor.b, 0) :
                             new Color(startColor.r, startColor.g, startColor.b, 1);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / duration;
                spriteRenderer.color = Color.Lerp(startColor, endColor, normalizedTime);
                yield return null;
            }

            spriteRenderer.color = endColor; // Ensure the final color is set
        }
    }

}
