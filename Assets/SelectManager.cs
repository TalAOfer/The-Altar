using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    private Card selectedCard = null;

    [SerializeField] private AllEvents events;
    private ActiveEffect currentAsker;

    [SerializeField] private HandManager handManager;
    [SerializeField] private Transform askerTransform;
    [SerializeField] private Transform selectTransform;
    public Button selectButton;

    public void WaitForActiveChoice(Component sender, object data)
    {
        ActiveEffect asker = (ActiveEffect)data;
        Card selectingCard = asker.parentCard;
        if (asker == null)
        {
            Debug.Log("No active effect was sent from " + sender.name);
            return;
        }

        handManager.ChangeHandState(HandState.Select);
        //if (currentAsker != null)
        //{ 
        //    StartCoroutine(currentAsker.parentCard.ChangeCardState(CardState.Selectable));
        //    Debug.Log("happened");
        //}
        //selectedCard = null;

        events.ToggleCurtain.Raise(this, true);
        selectButton.gameObject.SetActive(true);
        selectButton.interactable = false;

        handManager.RemoveCardFromHand(selectingCard);
        StartCoroutine(selectingCard.ChangeCardState(CardState.Selecting));
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
        handManager.AddCardToHand(selectedCard);
        yield return StartCoroutine(selectedCard.ChangeCardState(CardState.Default));

        Card selectingCard = currentAsker.parentCard;
        handManager.AddCardToHand(selectingCard);
        yield return StartCoroutine(selectingCard.ChangeCardState(CardState.Default));

        selectedCard = null;
        currentAsker = null;
    }

    public void OnCardClicked(Component sender, object data)
    {
        Card cardClicked = data as Card;
        if (cardClicked == selectedCard)
        {
            StartCoroutine(selectedCard.ChangeCardState(CardState.Selectable));
            handManager.InsertCardToHandByIndex(cardClicked, cardClicked.index);
            selectButton.interactable = false;
            selectedCard = null;
        }

        else
        {
            if (selectedCard != null)
            {
                StartCoroutine(selectedCard.ChangeCardState(CardState.Selectable));
                handManager.InsertCardToHandByIndex(selectedCard, selectedCard.index);
            }

            selectedCard = cardClicked;

            StartCoroutine(cardClicked.ChangeCardState(CardState.Selected));
            handManager.cardsInHand.Remove(cardClicked);
            StartCoroutine(cardClicked.interactionHandler.TransformCardUniformly
                (selectTransform.position, Vector3.one * GameConstants.SelectCardScale, Vector3.zero, 0.15f));

            selectButton.interactable = true;
        }

        handManager.ReorderCards();
    }




}
