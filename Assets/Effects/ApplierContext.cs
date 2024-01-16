using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplierContext
{
    public Card TargetCard { get; set; }
    public Card OtherCard { get; set; }


    public ApplierContext(Card targetCard, Card otherCard)
    {
        TargetCard = targetCard;
        OtherCard = otherCard;
    }
}
