using System;

public class Ability
{
    public AbilityType Type { get; private set; }
    public int TargetAmount { get; private set; }
    public SingleTargetRestriction TargetSingleRestriction { get; private set; }
    public TotalTargetRestriction TargetTotalRestriction { get; private set; }
    public InteractableType InteractableType { get; private set; }
    public int SingleRestrictionAmount { get; private set; }
    public Effect Effect { get; private set; }
    public Ability(AbilityBlueprint blueprint)
    {
        Type = blueprint.Type;
        TargetAmount = blueprint.TargetAmount;
        
        TargetSingleRestriction = blueprint.TargetSingleRestriction;
        SingleRestrictionAmount = blueprint.SingleRestrictionAmount;

        TargetTotalRestriction = blueprint.TargetTotalRestriction;
        InteractableType = blueprint.InteractableType;

        if (blueprint.NeedEffect && blueprint.EffectBlueprint != null)
        {
            Effect = blueprint.EffectBlueprint.InstantiateEffect(null, null);
        }
    }
}

public enum AbilityType
{
    Split,
    Merge,
    Paint
}

public enum SingleTargetRestriction
{
    None,
    BiggerThan,
    SmallerThan,
    Black,
    Red,
}

public enum TotalTargetRestriction
{
    None,
    SameColor,
    SumBiggerThan,
    SumSmallerThan,
}

[Flags]
public enum InteractableType
{
    PlayerCard = 1,
    EnemyCard = 2,
}