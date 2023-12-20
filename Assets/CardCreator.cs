using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCreator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private CardBlueprint cardBlueprint;
    [SerializeField] private Transform canvasTransform;

    [Button]
    public void SpawnCard()
    {
        GameObject cardGO = Instantiate(cardPrefab);
        cardGO.GetComponent<Card>().Init(cardBlueprint);
        cardGO.transform.SetParent(canvasTransform);
        cardGO.transform.SetParent(canvasTransform, false);

        RectTransform rectTransform = cardGO.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero; 
    }


}
