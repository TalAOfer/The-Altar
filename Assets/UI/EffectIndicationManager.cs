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

        Pooler.Spawn(indicatorPrefab, indication.effected.transform.position + offset, Quaternion.identity);
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
