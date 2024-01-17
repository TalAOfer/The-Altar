using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    private Card selectedCard = null;

    [SerializeField] private AllEvents events;
    private SelectEffect currentAsker;

    [SerializeField] private HandManager handManager;
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private Transform askerTransform;
    [SerializeField] private Transform selectTransform;
    public Button selectButton;

    public void WaitForActiveChoice(Component sender, object data)
    {
        SelectEffect asker = (SelectEffect)data;
        Card selectingCard = asker.parentCard;
        if (asker == null)
        {
            Debug.Log("No active effect was sent from " + sender.name);
            return;
        }

        handManager.ChangeHandState(HandState.Select);

        events.ToggleCurtain.Raise(this, true);
        selectButton.gameObject.SetActive(true);
        selectButton.interactable = false;

        handManager.RemoveCardFromHand(selectingCard);
        selectingCard.ChangeCardState(CardState.Selecting);
        StartCoroutine(selectingCard.interactionHandler.TransformCardUniformly(askerTransform.position, Vector3.one, Vector3.zero, 0.15f));
        events.SetGameState.Raise(this, GameState.SelectPlayerCard);
        currentAsker = asker;
    }

    public void ConfirmSelection()
    {
        StartCoroutine(currentAsker.HandleResponse(this, selectedCard));
    }

    public IEnumerator BringBackToDefault()
    {
        playerManager.SetAllCardState(CardState.Default);

        selectedCard = null;
        currentAsker = null;

        yield return null;
    }

    public void OnCardClicked(Component sender, object data)
    {
        Card cardClicked = data as Card;
        if (cardClicked == selectedCard)
        {
            selectedCard.ChangeCardState(CardState.Selectable);
            handManager.InsertCardToHandByIndex(cardClicked, cardClicked.index);
            selectButton.interactable = false;
            selectedCard = null;
        }

        else
        {
            if (selectedCard != null)
            {
                selectedCard.ChangeCardState(CardState.Selectable);
                handManager.InsertCardToHandByIndex(selectedCard, selectedCard.index);
            }

            selectedCard = cardClicked;

            cardClicked.ChangeCardState(CardState.Selected);
            handManager.cardsInHand.Remove(cardClicked);
            StartCoroutine(cardClicked.interactionHandler.TransformCardUniformly
                (selectTransform.position, Vector3.one * GameConstants.SelectCardScale, Vector3.zero, 0.15f));

            selectButton.interactable = true;
        }

        handManager.ReorderCards();
    }




}
