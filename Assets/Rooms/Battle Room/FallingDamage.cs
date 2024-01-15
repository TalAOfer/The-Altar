using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDamage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer digitSr;
    [SerializeField] private SpriteRenderer mathSymbolSr;
    [SerializeField] private SpriteFolder sprites;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float force;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float damageLerpDuration;
    [SerializeField] private float gravityAmount;


    private void OnEnable()
    {
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void Initialize(int damage)
    {
        bool isPositive = damage >= 0;
        int absDamage = Mathf.Abs(damage);
        if (absDamage > 10) absDamage = 10;
        mathSymbolSr.sprite = isPositive ? sprites.minus : sprites.plus;
        digitSr.sprite = sprites.digits[absDamage];
        
        mathSymbolSr.gameObject.SetActive(true);
        digitSr.gameObject.SetActive(true);
        
        rb.gravityScale = gravityAmount;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        Coroutine fadeSymbol = StartCoroutine(LerpColorCoroutine(mathSymbolSr, damageLerpDuration, true));
        Coroutine fadeDigit = StartCoroutine(LerpColorCoroutine(digitSr, damageLerpDuration, true)); 

        yield return fadeSymbol;
        yield return fadeDigit;

        Destroy(gameObject);
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
