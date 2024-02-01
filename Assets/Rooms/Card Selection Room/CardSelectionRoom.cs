using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEditor.PackageManager;
using UnityEngine;

public class CardSelectionRoom : Room
{
    [FoldoutGroup("Dependencies")]
    [SerializeField] private RoomData roomData;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private RunData runData;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;

    private FloorManager floorManager;

    [SerializeField] List<GameObject> texts;

    [SerializeField] private GameObject linkedCardsPrefab;

    [SerializeField] private Transform spawnContainer;

    [SerializeField] private float defaultSpacing;

    public List<LinkedCards> linkedCardsList = new();

    private Vector3 temp = new();

    [Button]
    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        base.InitializeRoom(floorManager, roomBlueprint);

        this.floorManager = floorManager;

        events.SetGameState.Raise(this, GameState.ChooseNewBlueprints);

        ToggleTexts(true);

        linkedCardsList.Clear();
        SpawnLinkedCards(roomBlueprint);
    }
    public override void OnRoomFinishedLerping()
    {
        base.OnRoomFinishedLerping();

        foreach (LinkedCards linkedCards in linkedCardsList)
        {
            linkedCards.playerCard.movement.SetNewDefaultLocation(linkedCards.playerCard.transform.position, Vector3.one, Vector3.zero);
            linkedCards.enemyCard.movement.SetNewDefaultLocation(linkedCards.enemyCard.transform.position, Vector3.one, Vector3.zero);
        }
    }
    public override void OnRoomFinished()
    {
        base.OnRoomFinished();

        ToggleTexts(false);
    }

    public void SpawnLinkedCards(RoomBlueprint roomBlueprint)
    {
        int cardAmount = roomBlueprint.amountOfOptions;

        float startOffset = -defaultSpacing * (cardAmount - 1) / 2;

        linkedCardsList.Clear();

        for (int i = 0; i < cardAmount; i++)
        {
            temp = transform.position;
            temp.x = startOffset + i * defaultSpacing;

            GameObject linkedCardsGo = Instantiate(linkedCardsPrefab, temp, Quaternion.identity, transform);
            LinkedCards linkedCards = linkedCardsGo.GetComponent<LinkedCards>();
            linkedCards.Init(roomBlueprint, i);
            linkedCardsList.Add(linkedCards);
        }
    }


    private void ToggleTexts(bool enable)
    {
        foreach (GameObject text in texts)
        {
            text.SetActive(enable);
        }
    }

    public void HandleChoice(int index)
    {
        StartCoroutine(ChoiceRoutine(index));
    }

    public IEnumerator ChoiceRoutine(int index)
    {
        LinkedCards linkedCards = linkedCardsList[index];

        linkedCards.wasChosen = true;
        CardBlueprint chosenPlayerBlueprint = linkedCards.playerCard.currentOverride;
        CardBlueprint chosenEnemyBlueprint = linkedCards.enemyCard.currentOverride;

        runData.playerCodex.OverrideCard(chosenPlayerBlueprint);
        runData.enemyCodex.OverrideCard(chosenEnemyBlueprint);

        RemoveAllCards();
        events.HideTooltip.Raise(this, null);

        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        floorManager.NextRoom();
    }

    [Button]
    public void RemoveAllCards()
    {
        List<LinkedCards> linkedCardsToDelete = new(linkedCardsList);
        foreach (LinkedCards linkedCards in linkedCardsToDelete)
        {
            if (!linkedCards.wasChosen)
            {
                runData.playerPool.ReturnBlueprintToPool(linkedCards.playerCard.currentOverride);
                runData.enemyPool.ReturnBlueprintToPool(linkedCards.enemyCard.currentOverride);
            }

            linkedCardsList.Remove(linkedCards);

            Destroy(linkedCards.gameObject);
        }
    }

    private IEnumerator HandleAllShapeshiftsUntilStable()
    {
        bool changesOccurred;
        List<Card> allCards = new(roomData.PlayerManager.activeCards);

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


