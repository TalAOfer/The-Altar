using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualHandler : MonoBehaviour
{
    [SerializeField] private Palette palette;
    [SerializeField] CardVisualData data;
    private Material cardMaterial;
    private Material spritesMaterial;

    [FoldoutGroup("Components")]
    [SerializeField] private Card card;
    [FoldoutGroup("Components")]
    [SerializeField] private Animator anim;

    [FoldoutGroup("Sprites Dependencies")]
    [SerializeField] private Material shaderMaterial;
    [FoldoutGroup("Sprites Dependencies")]
    public SpriteFolder sprites;

    [FoldoutGroup("Falling Damage")]
    [SerializeField] private GameObject fallingDamagePrefab;
    [FoldoutGroup("Falling Damage")]
    [SerializeField] private Transform fallingDamageTransform;

    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] Transform damageTransform;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private SpriteRenderer damageDigit;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private SpriteRenderer damageSymbol;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private List<Sprite> slashAnimationSprites;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private float slashAnimationIntervals = 0.1f;

    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer cardSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer iconSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer numberSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer symbolSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer slashSr;


    #region Initialization
    public void Init(CardBlueprint blueprint, string startingSortingLayer)
    {
        InitializeCardMaterial();
        InitializeSpritesMaterial();
        InitializeDamageVisualizerPosition();

        SetNewCardVisual();
        SetSortingLayer(startingSortingLayer);
    }

    private void InitializeCardMaterial()
    {
        cardMaterial = new Material(shaderMaterial);
        cardSr.material = cardMaterial;
        cardMaterial.SetColor("_Color", palette.white);
        cardMaterial.SetColor("_Outline_Color", palette.darkPurple);
        cardMaterial.SetInt("_Outline_On", 1);
    }

    private void InitializeSpritesMaterial()
    {
        spritesMaterial = new Material(shaderMaterial);
        symbolSr.material = spritesMaterial;
        iconSr.material = spritesMaterial;
        numberSr.material = spritesMaterial;
    }

    private void InitializeDamageVisualizerPosition()
    {
        damageTransform.localPosition = card.cardOwner == CardOwner.Player ? data.playerDamageVisualizerPosition : data.enemyDamageVisualizerPosition;
    }

    #endregion

    #region Setters
    public void SetNumberSprites()
    {
        numberSr.sprite = sprites.numbers[card.points];
    }
    public void SetNewCardVisual()
    {
        SetSpritesColor();
        SetCardIcon();
        SetCardSymbol();
        SetNumberSprites();
    }

    private void SetCardIcon()
    {
        iconSr.sprite = card.currentOverride.cardSprite;
    }
    private void SetCardSymbol()
    {
        Sprite sprite = null;
        switch (card.cardOwner)
        {
            case CardOwner.Player:
                sprite = card.cardColor == CardColor.Red ? sprites.hearts : sprites.spades;
                break;
            case CardOwner.Enemy:
                sprite = card.cardColor == CardColor.Red ? sprites.diamonds : sprites.clubs;
                break;
        }

        symbolSr.sprite = sprite;
    }

    public void SetCardBGColor(Color color)
    {
        cardSr.color = color;
    }

    public void SetSpritesColor()
    {
        Color color = card.cardColor == CardColor.Black ? palette.darkPurple : palette.lightRed;
        spritesMaterial.SetColor("_Color", color);
    }

    public void SetSortingOrder(int index)
    {
        int calcIndex = index * 5;
        cardSr.sortingOrder = calcIndex;
        iconSr.sortingOrder = calcIndex + 1;
        numberSr.sortingOrder = calcIndex + 2;
        symbolSr.sortingOrder = calcIndex + 3;
        slashSr.sortingOrder= calcIndex + 4;
    }

    public void SetSortingLayer(string sortingLayerName)
    {
        cardSr.sortingLayerName = sortingLayerName;
        numberSr.sortingLayerName = sortingLayerName;
        symbolSr.sortingLayerName = sortingLayerName;
        iconSr.sortingLayerName = sortingLayerName;
        damageDigit.sortingLayerName = sortingLayerName;
        damageSymbol.sortingLayerName = sortingLayerName;
        slashSr.sortingLayerName = sortingLayerName;
    }

    #endregion

    public void Reveal()
    {
        anim.Play("Card_Reveal");
    }

    #region Vanish Effect

    public IEnumerator ToggleOverallVanish(bool toBlank)
    {
        Coroutine cardVanish = StartCoroutine(LerpVanish(cardMaterial, toBlank, data.overallFadeDuration, data.overallFadeCurve));
        Coroutine spritesVanish = StartCoroutine(LerpVanish(spritesMaterial, toBlank, data.overallFadeDuration, data.overallFadeCurve));
        yield return cardVanish;
        yield return spritesVanish;
    }

    public IEnumerator ToggleSpritesVanish(bool toBlank)
    {
        yield return StartCoroutine(LerpVanish(spritesMaterial, toBlank, data.spritesFadeDuration, data.spritesFadeCurve));
    }

    private IEnumerator LerpVanish(Material material, bool toTransparent, float duration, AnimationCurve fadeCurve)
    {
        float timeElapsed = 0f;
        float startValue = toTransparent ? -0.5f : 1f;
        float endValue = toTransparent ? 1f : -0.5f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float curveValue = fadeCurve.Evaluate(t); // This will give you a value based on the curve's shape
            // Now map the curveValue (0-1) to your desired range (-0.5 to 1)
            float value = Mathf.Lerp(startValue, endValue, curveValue);
            material.SetFloat("_Vanish", value);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set
        material.SetFloat("_Vanish", endValue);
    }

    #endregion

    #region Damage Visualizer

    public void SpawnFallingDamage(int damage)
    {
        StartCoroutine(HurtAnimation());

        GameObject fallingDamageGo = Instantiate(fallingDamagePrefab, fallingDamageTransform.position, Quaternion.identity);
        FallingDamage fallingDamage = fallingDamageGo.GetComponent<FallingDamage>();
        fallingDamage.Initialize(damage);
    }

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

    public IEnumerator HurtAnimation()
    {
        StartCoroutine(Animate(slashSr, slashAnimationSprites, slashAnimationIntervals));
        Vector3 startingScale = transform.localScale;
        Vector3 HeartbeatScale = startingScale * 1.2f;
        card.transform.localScale = HeartbeatScale;
        yield return new WaitForSeconds(0.15f);
        card.transform.localScale = startingScale;
    }

    public static IEnumerator Animate(SpriteRenderer sr, List<Sprite> sprites, float changeInterval)
    {
        foreach (Sprite sprite in sprites)
        {
            sr.sprite = sprite;
            yield return new WaitForSeconds(changeInterval);
        }
    }

    #endregion

}
