using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/Color Decision")]
public class ColorDecision : Decision
{
    public ApplierTarget whoToCheck;
    public CardColor isIt;
    public override bool Decide(ApplierContext context)
    {
        Card targetCard = whoToCheck is ApplierTarget.Target ? context.TargetCard : context.OtherCard;
        return targetCard.cardColor == isIt;
    }
}
