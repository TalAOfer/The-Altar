using UnityEngine;

[CreateAssetMenu(fileName = "InteractionSettings", menuName = "Tweener/Settings", order = 1)]
public class TweenerSettings : ScriptableObject
{
    [Header("Jiggle Settings")]
    public float jiggleDuration = 0.5f;
    public float jiggleStrength = 0.1f;
    public int jiggleVibrato = 10;
    public float jiggleElasticity = 1f;

    [Header("Bounce Settings")]
    public float bounceDuration = 0.5f;
    public Vector3 bouncePunch = new(0, 0.2f, 0);
    public int bounceVibrato = 10;
    public float bounceElasticity = 1f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.1f;
    public int shakeVibrato = 10;
    public float shakeRandomness = 90f;
}