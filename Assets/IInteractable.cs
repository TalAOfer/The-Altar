using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool IsSelected { get; set; }
    public void UpdateSelectabilityVisual();

    public bool CanBeTarget { get; set; }
    public bool CanBeAgent { get; set; }

    [Flags]
    public enum InteractableType
    {
        PlayerCard = 1,
        EnemyCard = 2,
        Ability = 4,
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

public class Interaction : ScriptableObject
{
    public InteractableType TargetTypes;
    
    public int MinAmountOfTargets;
    public int MaxAmountOfTargets;
    public bool ShouldWaitForConfirm;
}

public class InteractionHandler : MonoBehaviour
{
    public Interaction interaction;
    protected List<Interactable> _targetsSelected;

    public void AddInteractable(Interactable interactable)
    {
        if (_targetsSelected.Count >= interaction.MaxAmountOfTargets)
        {
            _targetsSelected.Remove(_targetsSelected[^1]);
        }

        _targetsSelected.Add(interactable);

        if (!interaction.ShouldWaitForConfirm) Interact();
    }
    public  void Interact()
    {

    }
}