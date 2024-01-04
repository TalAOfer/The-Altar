using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlePointModifier
{
    public ModifierType modifierType;

    public float amount;

    public BattlePointModifier(ModifierType modifierType, float amount)
    {
        this.modifierType = modifierType; 
        this.amount = amount;
    }

    public int Apply(int currentPoints)
    {
        int returnInt = -1;
        switch (modifierType)
        {
            case ModifierType.Addition:
                returnInt = currentPoints + (int) amount;
                break;
            case ModifierType.Mult:
                returnInt = Tools.MultAndRoundUp(currentPoints, amount);
                break;
            case ModifierType.Replace:
                returnInt = (int)amount;
                break;
        }

        return returnInt;
    }
}
