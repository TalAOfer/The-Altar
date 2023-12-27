using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public bool isDead { get; private set; }

    [SerializeField] private Animator anim;

    public SpriteFolder sprites;

    public SpriteRenderer cardSr;

    public SpriteRenderer iconSr;
    public SpriteRenderer numberSr;
    public SpriteRenderer symbolSr;

    public int points;
    public int hurtPoints;
    public int attackPoints;

    public CardColor cardColor;
    public CardState cardState { get; private set; }
    public int index { get; private set; }

    [SerializeField] private float fadeLerpTime;
    [SerializeField] private ShapeshiftHelper shapeshiftHelper;

    public List<Effect> BeforeBattleEffects = new();
    public List<Effect> OnGainPointsEffects = new();
    public List<Effect> OnDeathEffects = new();

    public List<Effect> HurtPointsAlterationEffects = new();
    public List<Effect> AttackPointsAlterationEffects = new();

    public void Init(CardBlueprint blueprint, CardState cardState, int index, string startingSortingLayer)
    {
        this.cardState = cardState;
        this.index = index;

        SpawnEffects(blueprint);

        symbolSr.sprite = GetSymbol(blueprint.symbol);
        iconSr.sprite = blueprint.cardSprite;

        points = blueprint.defaultPoints;
        numberSr.sprite = GetNumberSprite(points);

        SetSortingLayer(startingSortingLayer);

        if (cardState != CardState.Reward)
        {
            SetColor(blueprint.cardColor);
        }

        if (cardState == CardState.Reward) anim.Play("Card_UpsideDown");
        else anim.Play("Card_Idle");
    }

    public void SpawnEffects(CardBlueprint blueprint)
    {
        foreach (EffectBlueprint effect in blueprint.BeforeBattle)
        {
            effect.SpawnEffect(EffectTrigger.BeforeBattle, this);
        }

        foreach (EffectBlueprint effect in blueprint.OnGainPoints)
        {
            effect.SpawnEffect(EffectTrigger.OnGainPoints, this);
        }

        foreach (EffectBlueprint effect in blueprint.OnDeath)
        {
            effect.SpawnEffect(EffectTrigger.OnDeath, this);
        }
    }

    public IEnumerator RemoveEffects()
    {
        // Create lists to hold the effects to be removed
        List<Effect> beforeBattleToRemove = new List<Effect>(BeforeBattleEffects);
        List<Effect> onGainPointsToRemove = new List<Effect>(OnGainPointsEffects);
        List<Effect> onDeathToRemove = new List<Effect>(OnDeathEffects);

        // Remove and destroy the before battle effects
        foreach (Effect effect in beforeBattleToRemove)
        {
            BeforeBattleEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Remove and destroy the on gain points effects
        foreach (Effect effect in onGainPointsToRemove)
        {
            OnGainPointsEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Remove and destroy the on death effects
        foreach (Effect effect in onDeathToRemove)
        {
            OnDeathEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Wait for the next fixed frame update to ensure all Destroy calls have been processed
        yield return new WaitForFixedUpdate();
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

    public IEnumerator CalcAttackPoints()
    {
        attackPoints = points;

        foreach (Effect effect in AttackPointsAlterationEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, null, EffectTrigger.AttackPointsAlteration)));
        }

        Debug.Log("calc attack points");
        yield return null;
    }

    public IEnumerator CalcHurtPoints(int damagePoints)
    {
        hurtPoints = damagePoints;

        foreach (Effect effect in HurtPointsAlterationEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, null, EffectTrigger.HurtPointsAlteration)));
        }

        Debug.Log("calc hurt points");
        yield return null;
    }

    public void TakeDamage()
    {
        points -= hurtPoints;
        if (points < 0) points = 0;
        numberSr.sprite = GetNumberSprite(points);
    }


    [Button]
    public void GiveDamage()
    {
        hurtPoints = 2;
        TakeDamage();
        StartCoroutine(Shapeshift());
    }

    public IEnumerator GainPoints(int pointsToGain)
    {
        points += pointsToGain;
        if (points < 0) points = 0;
        else if (points > 10) points = 10;
        numberSr.sprite = GetNumberSprite(points);

        yield return StartCoroutine(ApplyOnGainPointsEffects());
    }

    public IEnumerator ApplyOnGainPointsEffects()
    {
        foreach (Effect effect in OnGainPointsEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, null, EffectTrigger.OnGainPoints)));
        }
    }

    public IEnumerator HandleShapeshift()
    {
        if (points == 0) yield return StartCoroutine(TurnToBones());
        else yield return StartCoroutine(Shapeshift());
    }

    public IEnumerator TurnToBones()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(points, cardColor);
        SetNewCardArchetype(newForm);
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(ApplyOnDeathEffects());
    }

    public IEnumerator ApplyOnDeathEffects()
    {
        foreach (Effect effect in OnDeathEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, null, EffectTrigger.OnDeath)));
        }

        if (points != 0) StartCoroutine(Shapeshift()); 
        isDead = (points == 0);
        yield return null;
    }

    public IEnumerator Shapeshift()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(points, cardColor);
        SetNewCardArchetype(newForm);
        yield return StartCoroutine(RemoveEffects());
        SpawnEffects(newForm);
        yield return new WaitForSeconds(1);
    }

    public void SetColor(CardColor newColor)
    {
        cardColor = newColor;
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

    public IEnumerator ApplyBeforeBattleEffects(Card enemyCard)
    {
        foreach (Effect effect in BeforeBattleEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, enemyCard, EffectTrigger.BeforeBattle)));
        }
    }
}

public enum CardState
{
    Reward,
    Enemy,
    Hand
}
