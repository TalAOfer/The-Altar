using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool IsSelected { get; set; }
    public void UpdateSelectabilityVisual();

    public bool CanBeTarget { get; set; }
    public bool CanBeAgent { get; set; }

    public enum InteractableType
    {
        PlayerCard,
        EnemyCard,
        Ability
    }

}

public interface IActor
{
    public enum InteractionType
    {
        Fight,
        UseEffect
    }
}

public class Interaction
{
    public List<Interactable> targets;
    public int maxAmountOfTargets;
    public int minAmountOfTargets;
    public bool shouldWaitForConfirm;

    public void AddInteractable(Interactable interactable)
    {
        if (targets.Count >= maxAmountOfTargets)
        {
            targets.Remove(targets[^1]);
        }

        targets.Add(interactable);
    }
    public void Interact()
    {

    }
}
