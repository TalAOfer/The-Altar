using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectIndicationManager : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Vector3 offset;
    public void SpawnIndication(Component sender, object data)
    {
        EffectIndication indication = (EffectIndication) data;

        GameObject go = Pooler.Spawn(indicatorPrefab, indication.effected.transform.position + offset, Quaternion.identity);
        go.GetComponentInChildren<TextMeshProUGUI>().text = indication.effect;
        CustomAnimator animator = go.GetComponent<CustomAnimator>();
        animator.PlayAnimation("Fadeout");
        animator.PlayAnimation("Float");
    }
}

public class EffectIndication
{
    public string effect;
    public Card effected;
    public EffectIndication(string effect, Card effected) 
    {
        this.effect = effect;
        this.effected = effected;
    }   
} 
