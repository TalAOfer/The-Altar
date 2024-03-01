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
        Vector2Int hp_MaxHp = (Vector2Int)data;
        UpdateHealthUI(hp_MaxHp);
    }

    [Button]
    public void UpdateHealthUI(Vector2Int hp_maxHp)
    {
        int currentHp = hp_maxHp.x;
        int currentMaxHp = hp_maxHp.y;
        _text.text = currentHp.ToString();
        _fill.fillAmount = (float)currentHp / (float)currentMaxHp;
    }
}
