using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplierContext
{
    public Card EffectedCard { get; set; }
    public Card EffectingCard { get; set; }
    
    public ApplierContext(Card effectedCard, Card effectingCard)
    {
        EffectedCard = effectedCard;
        EffectingCard = effectingCard;
    }
}
