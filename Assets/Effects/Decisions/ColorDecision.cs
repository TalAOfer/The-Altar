using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/Color Decision")]
public class ColorDecision : Decision
{
    public CardColor isOther;
    public override bool Decide(EffectContext context)
    {
        Debug.Log(context.OtherCard.cardColor == isOther);
        return context.OtherCard.cardColor == isOther;
    }
}
