using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName ="Blueprints/Ability")]
public class AbilityBlueprint : ScriptableObject
{
    public Sprite icon;
    public AbilityType Type;
    public SingleTargetRestriction TargetSingleRestriction;
    [ShowIf("@ShouldShowSingleRestrictionAmount()")]
    public int SingleRestrictionAmount;
    public TotalTargetRestriction TargetTotalRestriction;
    public InteractableType InteractableType = InteractableType.PlayerCard;

    public int TargetAmount = 1;
    public bool NeedEffect;
    [ShowIf("NeedEffect")]
    public EffectBlueprint EffectBlueprint;

    private bool ShouldShowSingleRestrictionAmount()
    {
        return TargetSingleRestriction is SingleTargetRestriction.BiggerThan or SingleTargetRestriction.SmallerThan;
    }
}
