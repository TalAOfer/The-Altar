using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectContext
{
    public Card InitiatingCard { get; set; }
    public Card OtherCard { get; set; }


    public EffectContext(Card initiatingCard, Card otherCard)
    {
        InitiatingCard = initiatingCard;
        OtherCard = otherCard;
    }
}
