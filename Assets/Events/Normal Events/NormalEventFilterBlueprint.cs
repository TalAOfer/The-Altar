using Sirenix.OdinInspector;
using System;

[Serializable]
public class NormalEventFilterBlueprint
{
    public bool ShouldFilter = false;
    [ShowIf("ShouldFilter")]
    public NormalEventObjectFilterBlueprint Filter;

    public string Description => GetDescription();
    public string GetDescription()
    {
        string description = "";

        if (ShouldFilter)
        {
            if (Filter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                description += Filter.AffinityComparison.ToString().ToLower();
                description += " ";
            }

            if (Filter.DecisionType.HasFlag(DecisionType.Color))
            {
                description += Filter.PredefinedColor.ToString().ToLower();
                description += " ";
            }

            if (Filter.DecisionType.HasFlag(DecisionType.Parity))
            {
                description += Filter.PredefinedParity.ToString().ToLower();
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