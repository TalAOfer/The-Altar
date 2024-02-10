using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Title("Shake Types: Intensity/ Frequency/ Time")]
    [SerializeField] private Vector3 ClassicShake;

    private CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin perlin;

    private void Awake()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    [Button]
    public void TestShake(CameraShakeTypes shakeType)
    {
        InitiateCameraShake(this, shakeType);
    }

    public void InitiateCameraShake(Component sender, object data)
    {
        CameraShakeTypes shakeType = (CameraShakeTypes)data;

        float shakeIntensity = 1f;
        float shakeFrequency = 5f;
        float shakeTime = 0.25f;

        switch (shakeType)
        {
            case CameraShakeTypes.Classic:
                shakeIntensity = ClassicShake.x;
                shakeFrequency = ClassicShake.y;
                shakeTime = ClassicShake.z;
                break;
        }

        StartCoroutine(ShakeCameraRoutine(shakeIntensity, shakeFrequency, shakeTime));
    }

    private IEnumerator ShakeCameraRoutine(float intensity, float frequency, float time)
    {
        float startingIntensity = intensity;
        float shakeTimerTotal = time;
        float shakeTimer = time;

        perlin.m_FrequencyGain = frequency;
        perlin.m_AmplitudeGain = intensity;

        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            perlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            yield return null; // Wait until next frame
        }

        // Reset the perlin values or do any final adjustments here
        perlin.m_AmplitudeGain = 0f;
    }
}

public enum CameraShakeTypes
{
    Classic
}