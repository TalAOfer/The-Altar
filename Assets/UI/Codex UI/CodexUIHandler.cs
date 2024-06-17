using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodexUIHandler : MonoBehaviour
{
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private List<CodexUISlot> blackSlots;
    [SerializeField] private List<CodexUISlot> redSlots;
    [SerializeField] private SpriteRegistry spadesRegistry;
    [SerializeField] private SpriteRegistry heartsRegistry;

    private Vector3 offset = new(0, 0.6f, 0);
    private LevelBlueprint CurrentLevel => currentLevel.Value;

    public void Initialize()
    {
        foreach (CardBlueprint codexCardBlueprint in CurrentLevel.PlayerCards)
        {
            if (codexCardBlueprint != null)
            {

                CardArchetype archetype = codexCardBlueprint.Archetype;
                List<CodexUISlot> slotList = archetype.color is CardColor.Black ? blackSlots : redSlots;
                int slotIndex = archetype.points - 1;
                CodexUISlot slot = slotList[slotIndex];

                Card card = CardSpawner.Instance.SpawnCard(codexCardBlueprint, slot.transform.position - offset, slot.transform,
                    GameConstants.TOP_PLAYER_CARD_LAYER, CardInteractionType.Codex);

                card.visualHandler.SetSortingOrder(archetype.points);
            }
        }
    }

    [Button]
    public void InitializePlaceholder()
    {
        for (int i = 0; i < blackSlots.Count; i++)
        {
            CodexUISlot slot = blackSlots[i];
            slot.SetSortingOrder(i + 1);
            slot.SetPlaceholderSprite(spadesRegistry.sprites[i]);
        }

        for (int i = 0; i < redSlots.Count; i++)
        {
            CodexUISlot slot = redSlots[i];
            slot.SetSortingOrder(i + 1);
            slot.SetPlaceholderSprite(heartsRegistry.sprites[i]);
        }
    }


}
