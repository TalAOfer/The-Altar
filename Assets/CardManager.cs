using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public SpriteRenderer curtainSr;
    public BoxCollider2D curtainColl;

    public Transform topBattleTransform;
    public Transform bottomBattleTransform;

    private Card attackedCard;
    private Card attackingCard;

    public Button battleButton;
    public void OnCardDroppedOnCard(Component sender, object data)
    {
        attackedCard = sender as Card;
        attackingCard = data as Card;

        attackedCard.SetSortingLayer(GameConstants.BOTTOM_BATTLE_LAYER);
        attackingCard.SetSortingLayer(GameConstants.TOP_BATTLE_LAYER);
        
        attackedCard.transform.position = topBattleTransform.position;
        attackingCard.transform.position = bottomBattleTransform.position;
        attackingCard.transform.localScale = Vector3.one * 1.25f;
        attackedCard.transform.localScale = Vector3.one * 1.25f;

        battleButton.gameObject.SetActive(true);

        curtainColl.enabled = true;
        StartColorLerp(curtainSr, 0.5f, 0.7f);
    }

    public void DoBattle() 
    {
        int attackedCardDamage = attackedCard.points;
        int attackingCardDamage = attackingCard.points;
        attackedCard.TakeDamage(attackingCardDamage);
        attackingCard.TakeDamage(attackedCardDamage);
    }

    public void StartColorLerp(SpriteRenderer spriteRenderer, float duration, float to)
    {
        StartCoroutine(LerpColorCoroutine(spriteRenderer, duration, to));
    }

    private IEnumerator LerpColorCoroutine(SpriteRenderer spriteRenderer, float duration, float to)
    {
        if (spriteRenderer != null)
        {
            float elapsed = 0;
            Color startColor = spriteRenderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, to);

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
