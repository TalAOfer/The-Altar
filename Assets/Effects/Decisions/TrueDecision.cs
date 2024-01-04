using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/True Decision")]
public class TrueDecision : Decision
{
    public override bool Decide(EffectContext context)
    {
        return true;
    }
}