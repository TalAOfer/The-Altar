using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    private bool isCurtainOn = false;
    public void OnToggleCurtain(Component sender, object data)
    {
        StartCoroutine(ToggleCurtain((bool)data));
    }

    private IEnumerator ToggleCurtain(bool enable)
    {
        if (isCurtainOn == enable) yield break;
        float curtainAlpha = enable ? 0.7f : 0f;
        yield return StartCoroutine(LerpColorCoroutine(sr, 0.5f, curtainAlpha));
        isCurtainOn = enable;
    }

    private IEnumerator LerpColorCoroutine(SpriteRenderer spriteRenderer, float duration, float to)
    {
        if (spriteRenderer != null)
        {
            float elapsed = 0;
            Color startColor = spriteRenderer.color;
            Color endColor = new(startColor.r, startColor.g, startColor.b, to);

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
