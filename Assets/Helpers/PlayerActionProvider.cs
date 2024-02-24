using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionProvider : MonoBehaviour
{
    private PlayerManager playerManager;
    private void Awake()
    {
        playerManager = Locator.PlayerManager;
    }

    public void DrawCardToHand()
    {
        playerManager.DrawCardToHand();
    }

    public void SpawnCardToHandByArchetype(CardArchetype archetype)
    {
        playerManager.SpawnCardToHandByArchetype(archetype);
    }

    public void FillHand()
    {
        playerManager.FillHandToMinimum();
    }

    public IEnumerator HandleAllShapeshiftsUntilStable()
    {
        bool changesOccurred;
        List<Card> allCards = new(playerManager.ActiveCards);
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
