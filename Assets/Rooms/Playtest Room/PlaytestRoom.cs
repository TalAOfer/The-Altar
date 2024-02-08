using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaytestRoom : Room
{
    public GameObject cardPrefab;
    public RunData runData;
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

    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        this.roomBlueprint = roomBlueprint;
        this.floorManager = floorManager;
        cardInteraction.gameObject.SetActive(false);

        HandleInitialText();
        SpawnCard();
    }

    private Card SpawnCard()
    {
        BlueprintPoolInstance codex = roomBlueprint.affinity is CardOwner.Player ? runData.playerCodex : runData.enemyCodex;

        CardBlueprint blueprintToGain = roomBlueprint.cardBlueprint;
        CardBlueprint currentBlueprint = codex.GetCardOverride(new CardArchetype(blueprintToGain.defaultPoints, blueprintToGain.cardColor));
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
        string text = (roomBlueprint.affinity is CardOwner.Player) ? playerInitialText : enemyInitialText;
        string cardName = Tools.GetCardNameByArchetype(new CardArchetype(blueprintToGain.defaultPoints, blueprintToGain.cardColor), roomBlueprint.affinity);
        text = string.Format(text, cardName);
        textUGUI.text = text;
    }

    private void HandleExtraText()
    {
        textUGUI.text += "\n\n";
        textUGUI.text += (roomBlueprint.affinity is CardOwner.Player) ? playerExtraText : enemyExtraText;
    }

    public override void OnRoomFinishedLerping()
    {
        StartCoroutine(ChangeRoutine());
        cardInteraction.gameObject.SetActive(true);
    }

    private IEnumerator ChangeRoutine()
    {
        BlueprintPoolInstance codex = roomBlueprint.affinity is CardOwner.Player ? runData.playerCodex : runData.enemyCodex;

        codex.OverrideCard(roomBlueprint.cardBlueprint);

        StartCoroutine(HandleAllShapeshiftsUntilStable());
        yield return StartCoroutine(card.HandleShapeshift());
        if (roomBlueprint.shouldShowExtraText)
        {
            yield return new WaitForSeconds(extraTextDelay);
            HandleExtraText();
            yield return new WaitForSeconds(enableButtonDelay);
        }

        button.gameObject.SetActive(true);

    }

    public void NextRoom()
    {
        cardInteraction.gameObject.SetActive(false);
        floorManager.NextRoom();
    }

    private IEnumerator HandleAllShapeshiftsUntilStable()
    {
        bool changesOccurred;
        List<Card> allCards = new(roomData.PlayerManager.activeCards);
        if (allCards.Count <= 0) yield break;

        do
        {
            changesOccurred = false;
            List<Coroutine> shapeshiftCoroutines = new List<Coroutine>();

            foreach (Card card in allCards)
            {
                if (card.ShouldShapeshift())
                {
                    changesOccurred = true;
                    Coroutine coroutine = StartCoroutine(card.HandleShapeshift());
                    shapeshiftCoroutines.Add(coroutine);
                }
            }

            // Wait for all shapeshift coroutines to finish
            foreach (Coroutine coroutine in shapeshiftCoroutines)
            {
                yield return coroutine;
            }

            // If changesOccurred is true, the loop will continue
        } while (changesOccurred);

        // All shapeshifts are done and no more changes, proceed with the next operation
        // ...
    }
}
