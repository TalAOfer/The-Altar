using System.Collections;

public class ToggleColorEffect : Effect
{
    public ToggleColorEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        CardColor currentColor = target.cardColor;
        CardColor newColor = currentColor is CardColor.Black ? CardColor.Red : CardColor.Black;
        target.cardColor = newColor;
        RaiseEffectAppliedEvent(target, 0);
        yield break;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        CardColor newColor = target.cardColor;

        return "Change to " + newColor.ToString().ToLower();
    }
}
