using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Codex : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RunData runData;
    [SerializeField] private Vector2 offset;

    public Card SpawnCard(CardBlueprint cardBlueprint, Vector3 position, BlueprintPoolInstance codex)
    {
        GameObject cardGO = Instantiate(cardPrefab, position, Quaternion.identity, transform);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(codex, cardBlueprint, GameConstants.PLAYER_CARD_LAYER, CardInteractionType.Codex);

        return card;
    }

    [Button]
    public void Initialize()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = transform.position;
            pos.y -= i * offset.y;
            SpawnCard(runData.playerCodex.black[i], pos, runData.playerCodex);
        }
    }


}
