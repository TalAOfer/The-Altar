using UnityEngine;

public class EffectIndicationManager : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Vector3 offset;
    public void SpawnIndication(Component sender, object data)
    {
        EffectIndication indication = (EffectIndication) data;

        EffectIndicator indicator = Pooler.Spawn(indicatorPrefab, indication.effected.transform.position + offset, Quaternion.identity).GetComponent<EffectIndicator>();

        indicator.Initialize(indication);

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
