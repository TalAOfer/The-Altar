using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public enum InteractableType
{
    PlayerCard = 1,
    EnemyCard = 2,
}
public enum SingleTargetRestriction
{
    None,
    BiggerThan,
    SmallerThan,
    Black,
    Red,
}

public enum TargetTotalRestriction
{
    SameColor,
    SumBiggerThan,
    SumSmallerThan,
}


public class InteractionActorBlueprint : ScriptableObject
{
    public string Instruction;
    public EffectBlueprintReference EffectBlueprint;
    public InteractableType TargetTypes;
    public SingleTargetRestriction TargetRestriction;
    public int MinAmountOfTargets;
    public int MaxAmountOfTargets;
    public bool ShouldWaitForConfirm;
}

public class InteractionHandler : MonoBehaviour
{
    public Effect effect;
    public InteractionActorBlueprint interaction;
    protected List<Interactable> _targetsSelected;

    public void Initialize(EffectBlueprintReference effectBlueprint, BattleRoomDataProvider data)
    {
        effect = effectBlueprint.Value.InstantiateEffect(null, null, data);
    }

    public void ShowTargets()
    {

    }

    public void RemoveInteractable(Interactable interactable)
    {
        _targetsSelected.Remove(interactable);
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

