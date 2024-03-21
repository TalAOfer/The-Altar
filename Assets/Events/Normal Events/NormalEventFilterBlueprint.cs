using Sirenix.OdinInspector;
using System;

[Serializable]
public class NormalEventFilterBlueprint
{
    public bool FilterByEmitter = false;
    [ShowIf("FilterByEmitter")]
    public NormalEventObjectFilterBlueprint EmitterFilter;
    [ShowInInspector]
    public string Descrtiption => GetDescription();
    public string GetDescription()
    {
        string description = "Whenever a ";

        description += GetCardFilterDescription();

        description += "card has {verb}";

        return description;
    }

    public string GetCardFilterDescription()
    {
        string description = "";

        if (FilterByEmitter)
        {
            if (EmitterFilter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                description += EmitterFilter.AffinityComparison.ToString().ToLower();
                description += " ";
            }

            if (EmitterFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                description += EmitterFilter.PredefinedColor.ToString().ToLower();
                description += " ";
            }

            if (EmitterFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                description += EmitterFilter.PredefinedParity.ToString().ToLower();
                description += " ";
            }
        }

        return description;
    }
}

[Serializable]
public class NormalEventObjectFilterBlueprint
{
    public DecisionType DecisionType;

    [ShowIf("@ShouldShowColor()")]
    public CardColor PredefinedColor;

    [ShowIf("@ShouldShowAffinityComparison()")]
    public AffinityComparison AffinityComparison;

    [ShowIf("@ShouldShowParity()")]
    public Parity PredefinedParity;

    #region Inspector Helpers
    private bool ShouldShowColor() => DecisionType.HasFlag(DecisionType.Color);
    private bool ShouldShowAffinityComparison() => DecisionType.HasFlag(DecisionType.Affinity);
    private bool ShouldShowParity() => DecisionType.HasFlag(DecisionType.Parity);

    #endregion
}