using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class HandChoiceHandler : MonoBehaviour
{
    [SerializeField] private AllEvents events;
    private ActiveEffect currentAsker;
    public void WaitForActiveChoice(Component sender, object data)
    {
        ActiveEffect asker = (ActiveEffect)data;
        if (asker == null)
        {
            Debug.Log("No active effect was sent from " + sender.name);
            return;
        }

        events.SetGameState.Raise(this, GameState.SelectPlayerCard);
        currentAsker = asker;
    }

    public void OnCardSelected(Component sender, object data)
    {
        Card selectedCard = (Card)data;
        StartCoroutine(currentAsker.HandleResponse(this, selectedCard));
        currentAsker = null;
    }
}
