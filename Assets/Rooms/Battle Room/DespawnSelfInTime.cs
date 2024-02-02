using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnSelfInTime : MonoBehaviour
{
    [SerializeField] private float time;

    private void OnEnable()
    {
        StartCoroutine(DespawnAfterDelay());
    }
    public IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(time);
        Pooler.Despawn(gameObject);
    }
}
