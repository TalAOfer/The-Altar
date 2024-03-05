using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public enum InteractableType
{
    PlayerCard = 1,
    EnemyCard = 2,
    Ability = 4,
}
public enum TargetRestriction
{
    None,
    Black,
    Red,
    SameColor,
    BiggerThan,
    SmallerThan,
}


public class Interaction : ScriptableObject
{
    public string Instruction;
    public EffectBlueprint EffectBlueprint;
    public InteractableType TargetTypes;
    public int MinAmountOfTargets;
    public int MaxAmountOfTargets;
    public bool ShouldWaitForConfirm;
}

public class InteractionHandler : MonoBehaviour
{
    public Interaction interaction;
    protected List<Interactable> _targetsSelected;

    public void ShowTargets()
    {

    }

    public void AddInteractable(Interactable interactable)
    {
        if (_targetsSelected.Count >= interaction.MaxAmountOfTargets)
        {
            _targetsSelected.Remove(_targetsSelected[^1]);
        }

        _targetsSelected.Add(interactable);

        ShowTargets();

        if (!interaction.ShouldWaitForConfirm) Interact();
    }
    public void Interact()
    {

    }


}

