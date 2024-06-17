using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualHandler : MonoBehaviour
{
    [SerializeField] private Palette palette;
    [SerializeField] CardData data;
    private Material cardMaterial;
    private Material spritesMaterial;

    [FoldoutGroup("Components")]
    [SerializeField] private Card card;
    [FoldoutGroup("Components")]
    [SerializeField] private Animator anim;
    [FoldoutGroup("Components")]
    [SerializeField] private Tweener tweener;
    [FoldoutGroup("Components")]
    [SerializeField] private TweenBlueprint jiggleBlueprint;

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
    [SerializeField] private GameObject damageVisualizer;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private TextMeshProUGUI damageAmount;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private GameObject armorVisualizer;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private TextMeshProUGUI armorAmount;
    private bool isArmorActive;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private GameObject mightVisualizer;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private TextMeshProUGUI mightAmount;
    private bool isMightActive;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private List<Sprite> slashAnimationSprites;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private float slashAnimationIntervals = 0.1f;
    [FoldoutGroup("Damage Visualizer")]
    [SerializeField] private ParticleSystem particles;

    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer cardSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer iconSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer numberSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer symbolSr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer overlaySr;
    [FoldoutGroup("Card Renderers")]
    [SerializeField] private SpriteRenderer slashSr;



    #region Initialization

    private void Awake()
    {
        InitializeCardMaterial();
        InitializeSpritesMaterial();
        SetSpritesColor();
    }
    public void Init(string startingSortingLayer, CardInteractionType interactionType)
    {
        SetNewCardVisual();
        SetSortingLayer(startingSortingLayer);

    }

    private void InitializeSpritesMaskInteraction(CardInteractionType interactionType)
    {
        if (interactionType is CardInteractionType.Codex)
        {

        }
    }

    private void InitializeCardMaterial()
    {
        cardMaterial = new Material(shaderMaterial);
        cardSr.material = cardMaterial;
        cardMaterial.SetColor("_Color", Color.white);
        cardMaterial.SetInt("_Outline_On", 0);
    }

    private void InitializeSpritesMaterial()
    {
        spritesMaterial = new Material(shaderMaterial);
        symbolSr.material = spritesMaterial;
        iconSr.material = spritesMaterial;
        numberSr.material = spritesMaterial;
    }

    public void EnableOutline(PaletteColor color)
    {
        cardMaterial.SetColor("_Outline_Color", palette.GetColorByEnum(color));
        cardMaterial.SetInt("_Outline_On", 1);
    }

    public void DisableOutline()
    {
        cardMaterial.SetInt("_Outline_On", 0);
    }

    public void DisableBuffVisuals()
    {
        mightVisualizer.SetActive(false);
        armorVisualizer.SetActive(false);
    }

    public void ToggleDarkOverlay(bool enable)
    {
        Color transparent = Color.white;
        transparent.a = 0;
        Color dark = Color.black;
        dark.a = 0.8f;

        overlaySr.color = enable ? dark : transparent; 
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
        iconSr.sprite = card.Mask.cardSprite;
    }
    private void SetCardSymbol()
    {
        Sprite sprite = null;
        switch (card.Affinity)
        {
            case Affinity.Player:
                sprite = card.cardColor == CardColor.Red ? sprites.hearts : sprites.spades;
                break;
            case Affinity.Enemy:
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
        int calcIndex = index * 7;
        cardSr.sortingOrder = calcIndex;
        iconSr.sortingOrder = calcIndex + 1;
        numberSr.sortingOrder = calcIndex + 2;
        symbolSr.sortingOrder = calcIndex + 3;
        overlaySr.sortingOrder = calcIndex + 4;
        slashSr.sortingOrder = calcIndex + 5;
    }

    public void SetSortingLayer(string sortingLayerName)
    {
        cardSr.sortingLayerName = sortingLayerName;
        numberSr.sortingLayerName = sortingLayerName;
        symbolSr.sortingLayerName = sortingLayerName;
        iconSr.sortingLayerName = sortingLayerName;
        overlaySr.sortingLayerName = sortingLayerName;
        slashSr.sortingLayerName = sortingLayerName;
    }

    public string GetSortingLayer()
    {
        return cardSr.sortingLayerName;
    }

    #endregion

    public void Jiggle()
    {
        tweener.TriggerTween(jiggleBlueprint);
    }

    public void FlashOut()
    {
        overlaySr.color = Color.white;
        overlaySr.DOFade(0, 0.5f).SetEase(Ease.InCubic);
    }

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
        yield return LerpVanish(spritesMaterial, toBlank, data.spritesFadeDuration, data.spritesFadeCurve);
    }

    private IEnumerator LerpVanish(Material material, bool toTransparent, float duration, AnimationCurve fadeCurve)
    {
        float startValue = toTransparent ? -0.5f : 1f;
        float endValue = toTransparent ? 1f : -0.5f;

        yield return DOVirtual.Float(startValue, endValue, duration, value =>
        {
            material.SetFloat("_Vanish", value);
        })
        .SetEase(fadeCurve).WaitForCompletion(); 
    }

    #endregion

    #region Damage Visualizer

    public void SpawnFallingDamage(int damage)
    {
        StartCoroutine(HurtAnimation());

        GameObject fallingDamageGo = Pooler.Spawn
            (fallingDamagePrefab, fallingDamageTransform.position, Quaternion.identity);
        FallingDamage fallingDamage = fallingDamageGo.GetComponent<FallingDamage>();
        fallingDamage.Initialize(damage);
    }

    public void EnableDamageVisual()
    {
        damageVisualizer.SetActive(true);
        damageAmount.text = card.attackPoints.value.ToString();
    }

    public void DisableDamageVisual()
    {
        damageVisualizer.SetActive(false);
    }

    public void HandleArmorVisual()
    {
        armorAmount.text = card.Armor.ToString();

        if (!isArmorActive && card.Armor > 0)
        {
            isArmorActive = true;
            armorVisualizer.SetActive(true);
        }
        else if (isArmorActive && card.Armor == 0)
        {
            isArmorActive = false;
            armorVisualizer.SetActive(false);
        }
    }

    public void HandleMightVisual()
    {
        mightAmount.text = card.Might.ToString();

        if (!isMightActive && card.Might > 0)
        {
            isMightActive = true;
            mightVisualizer.SetActive(true);
        }
        else if (isMightActive && card.Might == 0)
        {
            isMightActive = false;
            mightVisualizer.SetActive(false);
        }
    }

    public void InitiateParticleSplash()
    {
        particles.Play();
    }

    public IEnumerator HurtAnimation()
    {
        StartCoroutine(Animate(slashSr, slashAnimationSprites, slashAnimationIntervals));
        Vector3 startingScale = transform.localScale;
        Vector3 HeartbeatScale = startingScale * 1.2f;
        transform.localScale = HeartbeatScale;
        yield return Tools.GetWait(0.15f);
        transform.localScale = startingScale;
    }

    public static IEnumerator Animate(SpriteRenderer sr, List<Sprite> sprites, float changeInterval)
    {
        foreach (Sprite sprite in sprites)
        {
            sr.sprite = sprite;
            yield return Tools.GetWait(changeInterval);
        }
    }

    #endregion

}
