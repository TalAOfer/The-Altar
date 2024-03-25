using Sirenix.OdinInspector;
using System;

[Serializable]
public class EffectBlueprint
{
    [BoxGroup("Trigger")]
    public EffectTriggerAsset Trigger;

    [BoxGroup("Trigger")]
    [ShowIf("@Trigger?.TriggerArchetype == TriggerArchetype.GlobalAmountEvents")]
    public GlobalAmountEventFilterBlueprint GlobalAmountEventsFilter;

    [BoxGroup("Trigger")]
    [ShowIf("@Trigger?.TriggerArchetype == TriggerArchetype.LocalAmountEvents")]
    public LocalAmountEventFilterBlueprint LocalAmountEventsFilter;

    [BoxGroup("Trigger")]
    [ShowIf("@Trigger?.TriggerArchetype == TriggerArchetype.GlobalNormalEvents")]
    public NormalEventFilterBlueprint NormalEventsFilter;

    [BoxGroup("Base")]
    public EffectTypeAsset EffectTypeAsset;

    [BoxGroup("Description")]
    [ShowInInspector]
    [MultiLineProperty(3)]
    public string Descrtiption => GetDescription();

    [BoxGroup("Description")]
    public bool replacePlaceholders = true;
    //public bool shouldAnimate = true;

    [FoldoutGroup("Target Acquiring")]
    //[ShowIf("@EffectTypeAsset?.TypeArchetype == EffectTypeArchetype.Grantable")]
    public EffectTargetPool TargetPool;
    [FoldoutGroup("Target Acquiring")]
    //[ShowIf("@ShouldShowTargetStrategy()")]
    public EffectTargetStrategy TargetStrategy;
    [FoldoutGroup("Target Acquiring")]
    //[ShowIf("@ShouldShowAmountOfTargets()")]
    public int AmountOfTargets = 1;
    [FoldoutGroup("Target Acquiring")]
    //[ShowIf("@EffectTypeAsset?.TypeArchetype == EffectTypeArchetype.Grantable")]
    public bool ShouldFilterTargets;
    [FoldoutGroup("Target Acquiring")]
    [ShowIf("ShouldFilterTargets")]
    public NormalEventFilterBlueprint TargetFilterBlueprint;

    [FoldoutGroup("Effect")]
    public bool IsPersistent;

    [ShowIf("@ShouldShowAmountStrategy()")]
    [FoldoutGroup("Effect")]
    public GetAmountStrategy amountStrategy;

    [ShowIf("@ShouldShowAmount()")]
    [FoldoutGroup("Effect")]
    public int amount = 1;

    [FoldoutGroup("Effect")]
    [ShowIf("@ShouldShowCardArchetype()")]
    public CardArchetype cardArchetype;

    #region Battle Modifier

    [ShowIf("@GetCurrentEffectType()", EffectType.AddBattlePointModifier)]
    [FoldoutGroup("Effect")]
    public BattlePointType battlePointType;

    [ShowIf("@GetCurrentEffectType()", EffectType.AddBattlePointModifier)]
    [FoldoutGroup("Effect")]
    public ModifierType modifierType;

    [ShowIf("@GetCurrentEffectType()", EffectType.AddBattlePointModifier)]
    [FoldoutGroup("Effect")]
    public bool filterBattleModifier;

    [ShowIf("@ShouldShowBattleModifierFilter()")]
    [FoldoutGroup("Effect")]
    public BattleModifierFilterBlueprint battleModifierfilter;

    #endregion

    #region Set Color

    [ShowIf("@GetCurrentEffectType()", EffectType.SetColor)]
    [FoldoutGroup("Effect")]
    public CardColor color;

    #endregion

    #region Guardians

    [ShowIf("@GetCurrentEffectType()", EffectType.AddGuardian)]
    [FoldoutGroup("Effect")]
    public GuardianType guardianType;

    [ShowIf("@GetCurrentEffectType()", EffectType.AddGuardian)]
    [FoldoutGroup("Effect")]
    public bool guardianIsPresistent;

    #endregion

    #region Buff

    [ShowIf("@GetCurrentEffectType()", EffectType.GainBuff)]
    [FoldoutGroup("Effect")]
    public BuffType BuffType;

    #endregion

    #region Add Effect

    [ShowIf("@GetCurrentEffectType()", EffectType.AddEffect)]
    [FoldoutGroup("Effect")]
    public EffectBlueprintAsset effectBlueprint;

    #endregion


    public Effect InstantiateEffect(Card parentCard, BattleRoomDataProvider data)
    {
        return EffectTypeAsset.InstantiateEffect(this, parentCard, data);
    }

    #region Inspector Helpers

    private EffectType GetCurrentEffectType()
    {
        if (EffectTypeAsset == null)
        {
            return 0;
        }
        else
        {
            return EffectTypeAsset.Type;
        }
    }
    private bool ShouldShowAmount()
    {
        if (EffectTypeAsset != null)
        {
            return EffectTypeAsset.NeedsAmount
                && amountStrategy is GetAmountStrategy.Value;
        }
        else return false;
    }

    private bool ShouldShowCardArchetype()
    {
        if (EffectTypeAsset != null) return EffectTypeAsset.NeedsArchetype;
        else return false;
    }
    private bool ShouldShowAmountStrategy()
    {
        if (EffectTypeAsset != null) return EffectTypeAsset.NeedsAmount;
        else return false;
    }
    private bool ShouldShowAmountOfTargets()
    {
        return (TargetStrategy is EffectTargetStrategy.Random or EffectTargetStrategy.Highest or EffectTargetStrategy.Lowest)
            && ShouldShowTargetStrategy();
    }
    private bool ShouldShowTargetStrategy()
    {
        return EffectTypeAsset.TypeArchetype == EffectTypeArchetype.Grantable && TargetPool is EffectTargetPool.AllCards;
    }

    private bool ShouldShowTargetFilter()
    {
        return ShouldFilterTargets && ShouldShowTargetStrategy();
    }
    private bool ShouldShowBattleModifierFilter()
    {
        return GetCurrentEffectType() is EffectType.AddBattlePointModifier && filterBattleModifier;
    }

    #endregion

    #region Description Builder
    public string GetDescription()
    {
        //Trigger
        if (Trigger == null || EffectTypeAsset == null) return "";

        string triggerText = Trigger.IsRigid ? Trigger.TriggerName : Trigger.TriggerBaseText;
        if (Trigger.IsRigid) triggerText += ": ";


        //Effect
        string effectText = EffectTypeAsset.BaseEffectText;

        if (EffectTypeAsset.Type is EffectType.AddBattlePointModifier && filterBattleModifier)
        {
            effectText += " ";
            effectText += battleModifierfilter.GetDescription();
        }

        //Target

        string target = GetTargetString();

        string description = triggerText + ", " + effectText + " " + target;

        if (replacePlaceholders)
        {
            description = description.Replace("{amount}", GetAmountString());
            description = description.Replace("{normalCardFilter}", NormalEventsFilter.GetCardFilterDescription());


            if (Trigger.TriggerArchetype is TriggerArchetype.GlobalAmountEvents or TriggerArchetype.LocalAmountEvents)
            {
                string amountFilterText = Trigger.TriggerArchetype is TriggerArchetype.LocalAmountEvents ?
                    LocalAmountEventsFilter.GetAmountFilterDescription() : GlobalAmountEventsFilter.GetAmountFilterDescription();

                string objectFilterText = Trigger.TriggerArchetype is TriggerArchetype.LocalAmountEvents ?
                                                LocalAmountEventsFilter.GetCardFilterDescription() : GlobalAmountEventsFilter.GetCardFilterDescription();

                description = description.Replace("{amountCardFilter}", objectFilterText);
                description = description.Replace("{amountFilter}", amountFilterText);
            }



            description = description.Replace("{targetVerb}", GetTargetVerb());
            description = description.Replace("{buffType}", BuffType.ToString());
            description.Trim();
        }

        return description;
    }

    private string GetAmountString()
    {
        string amountString = "";

        if (amountStrategy is GetAmountStrategy.Value)
        {
            amountString = amount.ToString();
        }

        else
        {
            amountString = 1.ToString();
        }

        return amountString;
    }
    private string GetAmountStrategyString()
    {
        string amountString = "";

        switch (amountStrategy)
        {
            case GetAmountStrategy.Value:
                amountString = amount.ToString();
                break;
            case GetAmountStrategy.EmptySpacesOnMap:
                break;
            case GetAmountStrategy.EnemiesOnMap:
                amountString = "for each enemy on map";
                break;
            case GetAmountStrategy.NotImplementedDeadEnemiesOnMap:
                break;
            case GetAmountStrategy.CardsInHand:
                amountString = "for each card in your hand";
                break;
            case GetAmountStrategy.RoomCount:
                amountString = "for each room you encountered this floor";
                break;
            case GetAmountStrategy.LowestValueEnemyCard:
                amountString = "times the value of the lowest enemy card on map";
                break;
        }

        return amountString;
    }

    private string GetTargetString()
    {
        string strategy = "";

        if (EffectTypeAsset.TypeArchetype != EffectTypeArchetype.Grantable)
        {
            return "";
        }

        switch (TargetStrategy)
        {
            case EffectTargetStrategy.All:
                break; // No strategy text for "All" as it's implied
            case EffectTargetStrategy.Random:
                strategy = "random ";
                break;
            case EffectTargetStrategy.Highest:
                strategy = "highest ";
                break;
            case EffectTargetStrategy.Lowest:
                strategy = "lowest ";
                break;
        }


        string filter = "";
        
        if (ShouldFilterTargets)
        {
            filter = TargetFilterBlueprint.GetCardFilterDescription();
        }

        string pool = "";

        switch (TargetPool)
        {
            case EffectTargetPool.InitiatingCard:
                return ""; // If it's the initiating card, we assume no additional text is needed.
            case EffectTargetPool.Oppnent:
            case EffectTargetPool.SelectedCards:
                break; // No additional text for these cases in your original method
            case EffectTargetPool.AllCards:
                pool = "card";
                break;
        }

        string amountText = AmountOfTargets == 1 ? "" : AmountOfTargets.ToString() + " ";
        string theText = AmountOfTargets == 1 ? "the " : "";

        // Assemble the parts into the final string
        string targetString = "to ";


        // We use "the" only when there's a strategy and it's singular, e.g., "the highest"
        if (!string.IsNullOrEmpty(theText) && !string.IsNullOrEmpty(pool))
        {
            targetString += theText + strategy + filter + pool;
        }
        else
        {
            // If AmountOfTargets is greater than one or pool is empty, we don't use "the"
            targetString += amountText + strategy + filter + (AmountOfTargets > 1 ? pool + "s" : pool);
        }

        return targetString.Trim();
    }

    private string GetTargetVerb()
    {
        string targetVerb = "";

        if (EffectTypeAsset.TypeArchetype is EffectTypeArchetype.Grantable)
        {
            if (TargetPool is EffectTargetPool.InitiatingCard)
            {
                targetVerb = EffectTypeAsset.GainVerb;
            }
            else
            {
                targetVerb = EffectTypeAsset.GrantVerb;
            }
        }

        return targetVerb;
    }
    #endregion

}

public enum EffectTargetPool
{
    InitiatingCard,
    Oppnent,
    SelectedCards,
    AllCards,
    TriggerCard,
}

public enum EffectTargetStrategy
{
    All,
    Random,
    Highest,
    Lowest
}

public enum EffectType
{
    DebugEffect,
    AddBattlePointModifier,
    SetColor,
    ToggleColor,
    GainPoints,
    DrawCard,
    SpawnCardToHand,
    AddEffect,
    AddGuardian,
    SpawnEnemy,
    GainBuff,
    TakeDamage,
}

public enum GetAmountStrategy
{
    Value,
    EmptySpacesOnMap,
    EnemiesOnMap,
    NotImplementedDeadEnemiesOnMap,
    CardsInHand,
    RoomCount,
    LowestValueEnemyCard,
}

