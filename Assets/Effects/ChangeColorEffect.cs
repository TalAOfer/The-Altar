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

        string ColorChangedTo = ToggleCardColor(cardToChange);

        SendLog(cardToChange, ColorChangedTo);

        yield return new WaitForSeconds(postdelay);
    }

    private string ToggleCardColor(Card card)
    {
        if (card.cardColor == CardColor.Black)
        {
            card.cardColor = CardColor.Red;
            return "red";
        }
        else
        {
            card.cardColor = CardColor.Black;
            return "black";
        }
    }

    private void SendLog(Card changedCard, string colorChangedTo)
    {
        string log = changedCard.name + " changed to " + colorChangedTo;
        events.AddLogEntry.Raise(this, log);
    }
}

public enum WhoToChange 
{
    Initiating,
    Other
}
