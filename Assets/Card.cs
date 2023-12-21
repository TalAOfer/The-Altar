using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private bool startUpsideDown;
    [SerializeField] private Animator anim;

    public SpriteRenderer cardSr;
    public SpriteFolder sprites;
    public SpriteRenderer number;
    public SpriteRenderer symbol;
    public SpriteRenderer cardSprite;

    public int points;
    public CardColor cardColor;

    [SerializeField] private float fadeLerpTime;

    public void Init(CardBlueprint blueprint)
    {
        symbol.sprite = GetSymbol(blueprint.symbol);
        cardSprite.sprite = blueprint.cardSprite;

        points = blueprint.defaultPoints;
        number.sprite = GetNumberSprite(points);

        cardColor = blueprint.cardColor;
        
        if (!startUpsideDown)
        {
            SetColor(cardColor);
        }

        if (startUpsideDown) anim.Play("Card_UpsideDown");
        else anim.Play("Card_Idle");
    }

    public void Reveal()
    {
        anim.Play("Card_Reveal");
    }

 
    private void SetColor(CardColor cardColor)
    {
        Color color = cardColor == CardColor.Black ? Color.black : Color.red;
        number.color = color;
        symbol.color = color;
        cardSprite.color = color;
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
        StartColorLerp(symbol, fadeLerpTime, false);
        StartColorLerp(number, fadeLerpTime, false);
        StartColorLerp(cardSprite, fadeLerpTime, false);
    }

    public void FadeSprites()
    {

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

public enum CardStates
{
    UpsideDown,
    Revealed
}
