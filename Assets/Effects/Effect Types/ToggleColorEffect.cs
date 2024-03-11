using System.Collections;

public class ToggleColorEffect : Effect
{
    public ToggleColorEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        CardColor currentColor = target.cardColor;
        CardColor newColor = currentColor is CardColor.Black ? CardColor.Red : CardColor.Black;
        target.cardColor = newColor;
        RaiseEffectAppliedEvent(target, 0);
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        CardColor newColor = target.cardColor;

        return "Change to " + newColor.ToString().ToLower();
    }
}
