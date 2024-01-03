using System.Collections;
using UnityEngine;

public class ChangeColorEffect : Effect
{
    public WhoToChange whoToChange;

    public void Initialize(WhoToChange whoToChange)
    {
        this.whoToChange = whoToChange;
    }

    public override IEnumerator Apply(EffectContext context)
    {
        yield return new WaitForSeconds(predelay);

        Card cardToChange = whoToChange == WhoToChange.Initiating ? context.InitiatingCard : context.OtherCard;

        ToggleCardColor(cardToChange);

        yield return new WaitForSeconds(postdelay);
    }

    private void ToggleCardColor(Card card)
    {
        if (card.cardColor == CardColor.Black)
        {
            card.cardColor = CardColor.Red;
        }
        else
        {
            card.cardColor = CardColor.Black;
        }
    }
}

public enum WhoToChange 
{
    Initiating,
    Other
}
