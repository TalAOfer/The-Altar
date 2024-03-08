using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private RunData runData;
    [SerializeField] private Image _fill;
    [SerializeField] private TextMeshProUGUI _text;

    public void UpdateUI(Component sender, object data)
    {
        PlayerHealth playerHealth = (PlayerHealth)data;
        UpdateHealthUI(playerHealth);
    }

    [Button]
    public void UpdateHealthUI(PlayerHealth playerHealth)
    {
        _text.text = playerHealth.Current.ToString();
        _fill.fillAmount = (float)playerHealth.Current / (float)playerHealth.Max;
    }
}
