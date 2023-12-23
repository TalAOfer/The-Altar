using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public bool isDead {  get; private set; }

    [SerializeField] private Animator anim;

    public SpriteFolder sprites;

    public SpriteRenderer cardSr;

    public SpriteRenderer iconSr;
    public SpriteRenderer numberSr;
    public SpriteRenderer symbolSr;

    public int points;
    public CardColor cardColor;
   
    public CardState cardState { get; private set; }
    public int index { get; private set; }

    [SerializeField] private float fadeLerpTime;
    [SerializeField] private ShapeshiftHelper shapeshiftHelper;

    public void Init(CardBlueprint blueprint, CardState cardState, int index, string startingSortingLayer)
    {
        this.cardState = cardState;
        this.index = index; 

        symbolSr.sprite = GetSymbol(blueprint.symbol);
        iconSr.sprite = blueprint.cardSprite;

        points = blueprint.defaultPoints;
        numberSr.sprite = GetNumberSprite(points);

        cardColor = blueprint.cardColor;

        SetSortingLayer(startingSortingLayer);

        if (cardState != CardState.Reward)
        {
            SetColor(cardColor);
        }

        if (cardState == CardState.Reward) anim.Play("Card_UpsideDown");
        else anim.Play("Card_Idle");
    }

    public void SetNewCardArchetype(CardBlueprint blueprint)
    {
        symbolSr.sprite = GetSymbol(blueprint.symbol);
        iconSr.sprite = blueprint.cardSprite;

        points = blueprint.defaultPoints;
        numberSr.sprite = GetNumberSprite(points);

        cardColor = blueprint.cardColor;
        SetColor(cardColor);
    }

    public void Reveal()
    {
        anim.Play("Card_Reveal");
    }


    private void SetColor(CardColor cardColor)
    {
        Color color = cardColor == CardColor.Black ? Color.black : Color.red;
        numberSr.color = color;
        symbolSr.color = color;
        iconSr.color = color;
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

    public void TakeDamage(int damagePoints)
    {
        points -= damagePoints;
        if (points < 0) points = 0;
        numberSr.sprite = GetNumberSprite(points);
    }

    public IEnumerator Shapeshift()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(points, cardColor);
        SetNewCardArchetype(newForm);
        if (points == 0)
        {
            yield return StartCoroutine(OnDeath());
        }

        Debug.Log("happened");

        yield return null;
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

    public IEnumerator OnDeath()
    {
        ///if OnDeath.Lenght == 0;
        isDead = true;
        yield return null;
    }
}

public enum CardState
{
    Reward,
    Enemy,
    Hand
}
