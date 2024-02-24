using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaytestRoom : Room
{
    private PlayerActionProvider playerActions;
    public GameObject cardPrefab;
    public RunData runData;
    public FloorData floorData;
    [SerializeField] private RoomData roomData;
    private FloorManager floorManager;
    private RoomBlueprint roomBlueprint;
    [SerializeField] private CustomButton button;
    [SerializeField] private TextMeshProUGUI textUGUI;
    [TextArea(0, 3)]
    [SerializeField] private string playerInitialText;
    [SerializeField] private string playerExtraText;
    [TextArea(0, 3)]
    [SerializeField] private string enemyInitialText;
    [SerializeField] private string enemyExtraText;
    private Card card;
    [SerializeField] private float extraTextDelay;
    [SerializeField] private float enableButtonDelay;

    private void Awake()
    {
        playerActions = Locator.PlayerActionProvider;
    }

    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        this.roomBlueprint = roomBlueprint;
        this.floorManager = floorManager;
        cardInteraction.gameObject.SetActive(false);

        HandleInitialText();
        SpawnCard();
    }

    public override void OnRoomFinishedLerping()
    {
        StartCoroutine(ChangeRoutine());
        cardInteraction.gameObject.SetActive(true);
    }

    private Card SpawnCard()
    {
        Codex codex = roomBlueprint.affinity is Affinity.Player ? runData.playerCodex : floorData.enemyCodex;

        CardBlueprint blueprintToGain = roomBlueprint.cardBlueprint;
        CardBlueprint currentBlueprint = codex.GetCardOverride(new CardArchetype(blueprintToGain.archetype.points, blueprintToGain.archetype.color));
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
        cardGO.name = currentBlueprint.name;
        card = cardGO.GetComponent<Card>();
        card.Init(codex, currentBlueprint, GameConstants.ENEMY_CARD_LAYER, CardInteractionType.Playable);
        card.visualHandler.ToggleOutline(true);
        return card;
    }

    private void HandleInitialText()
    {
        CardBlueprint blueprintToGain = roomBlueprint.cardBlueprint;
        string text = (roomBlueprint.affinity is Affinity.Player) ? playerInitialText : enemyInitialText;
        string cardName = Tools.GetCardNameByArchetype(new CardArchetype(blueprintToGain.archetype.points, blueprintToGain.archetype.color), roomBlueprint.affinity);
        text = string.Format(text, cardName);
        textUGUI.text = text;
    }

    private void HandleExtraText()
    {
        textUGUI.text += "\n\n";
        textUGUI.text += (roomBlueprint.affinity is Affinity.Player) ? playerExtraText : enemyExtraText;
    }


    private IEnumerator ChangeRoutine()
    {
        Codex codex = roomBlueprint.affinity is Affinity.Player ? runData.playerCodex : floorData.enemyCodex;

        codex.OverrideCard(roomBlueprint.cardBlueprint);

        StartCoroutine(playerActions.HandleAllShapeshiftsUntilStable());
        yield return StartCoroutine(card.HandleShapeshift());
        if (roomBlueprint.shouldShowExtraText)
        {
            yield return Tools.GetWait(extraTextDelay);
            HandleExtraText();
            yield return Tools.GetWait(enableButtonDelay);
        }

        button.gameObject.SetActive(true);

    }

    public void NextRoom()
    {
        button.gameObject.SetActive(false);
        cardInteraction.gameObject.SetActive(false);
        floorManager.NextRoom();
    }
}
