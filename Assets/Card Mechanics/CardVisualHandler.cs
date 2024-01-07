using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CardVisualHandler : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Animator anim;
    [SerializeField] private Material shaderMaterial;

    [SerializeField] private GameObject fallingDamagePrefab;
    [SerializeField] private Transform fallingDamageTransform;

    private Material cardMaterial;
    private Material spritesMaterial;

    public SpriteFolder sprites;

    public SpriteRenderer cardSr;

    public SpriteRenderer iconSr;
    public SpriteRenderer numberSr;
    public SpriteRenderer symbolSr;

    public SpriteRenderer damageDigit;
    public SpriteRenderer damageSymbol;

    [SerializeField] private float overallFadeLerpDuration = 2f;
    [SerializeField] private float spritesFadeLerpDuration = 1.25f;
    [SerializeField] private ScriptableAnimationCurve fadeCurve;

    public void Init(CardBlueprint blueprint, string startingSortingLayer)
    {
        cardMaterial = new Material(shaderMaterial);
        cardSr.material = cardMaterial;
        cardMaterial.SetColor("_Color", Color.white);
        cardMaterial.SetColor("_Outline_Color", Color.black);
        cardMaterial.SetInt("_Outline_On", 1);

        spritesMaterial = new Material(shaderMaterial);
        symbolSr.material = spritesMaterial;
        iconSr.material = spritesMaterial;
        numberSr.material = spritesMaterial;

        SetSpritesColor(blueprint.cardColor);
        symbolSr.sprite = GetSymbol();
        iconSr.sprite = blueprint.cardSprite;
        numberSr.sprite = GetNumberSprite(card.points);
        SetSortingLayer(startingSortingLayer);
    }

    public void UpdateNumberSprite()
    {
        numberSr.sprite = GetNumberSprite(card.points);
    }

    public void SetNewCardVisual(CardBlueprint blueprint)
    {
        SetSpritesColor(card.cardColor);
        symbolSr.sprite = GetSymbol();
        iconSr.sprite = blueprint.cardSprite;
        numberSr.sprite = GetNumberSprite(card.points);
        UpdateNumberSprite();
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
        spritesMaterial.SetColor("_Color", color);
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
        damageDigit.sortingLayerName = sortingLayerName;
        damageSymbol.sortingLayerName = sortingLayerName;
    }

    private Sprite GetNumberSprite(int currentPoints)
    {
        return sprites.numbers[currentPoints];
    }

    private Sprite GetSymbol()
    {
        Sprite sprite = null;
        switch (card.cardOwner)
        {
            case CardOwner.Player:
                sprite = card.cardColor == CardColor.Red ? sprites.hearts : sprites.clubs;
                break;
            case CardOwner.Enemy:
                sprite = card.cardColor == CardColor.Red ? sprites.diamonds : sprites.spades;
                break;
        }

        return sprite;
    }

    public void SpawnFallingDamage(int damage)
    {
        GameObject fallingDamageGo = Instantiate(fallingDamagePrefab, fallingDamageTransform.position, Quaternion.identity);
        FallingDamage fallingDamage = fallingDamageGo.GetComponent<FallingDamage>();
        fallingDamage.Initialize(damage);
    }

    //public void StartColorLerp(SpriteRenderer spriteRenderer, float duration, bool toTransparent)
    //{
    //    StartCoroutine(LerpColorCoroutine(spriteRenderer, duration, toTransparent));
    //}


    //[Button]
    //public void StartVanishLerp()
    //{
    //    StartCoroutine(LerpVanish(cardMaterial, true));
    //    StartCoroutine(LerpVanish(spritesMaterial, true)); 
    //}

    public IEnumerator ToggleOverallVanish(bool toBlank)
    {
        Coroutine cardVanish = StartCoroutine(LerpVanish(cardMaterial, toBlank, overallFadeLerpDuration));
        Coroutine spritesVanish = StartCoroutine(LerpVanish(spritesMaterial, toBlank, overallFadeLerpDuration));
        yield return cardVanish;
        yield return spritesVanish;
    }

    public IEnumerator ToggleSpritesVanish(bool toBlank)
    {
        yield return StartCoroutine(LerpVanish(spritesMaterial, toBlank, spritesFadeLerpDuration));
    }

    private IEnumerator LerpVanish(Material material, bool toTransparent, float duration)
    {
        float timeElapsed = 0f;
        float startValue = toTransparent ? -0.5f : 1f;
        float endValue = toTransparent ? 1f : -0.5f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float curveValue = fadeCurve.curve.Evaluate(t); // This will give you a value based on the curve's shape
            // Now map the curveValue (0-1) to your desired range (-0.5 to 1)
            float value = Mathf.Lerp(startValue, endValue, curveValue);
            material.SetFloat("_Vanish", value);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set
        material.SetFloat("_Vanish", endValue);
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

    #region Damage Visualizer

    public void EnableDamageVisual(int amount)
    {
        damageDigit.gameObject.SetActive(true);
        damageSymbol.gameObject.SetActive(true);

        bool isDamage = amount >= 0;
        int absAmount = Mathf.Abs(amount);

        damageDigit.sprite = sprites.digits[absAmount];
        damageSymbol.sprite = isDamage ? sprites.damageIcon : sprites.regenIcon;
    }

    public void DisableDamageVisual()
    {
        damageSymbol.gameObject.SetActive(false);
        damageDigit.gameObject.SetActive(false);
    }

    #endregion

}
