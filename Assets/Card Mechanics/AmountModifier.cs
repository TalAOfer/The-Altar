using System;

[Serializable]
public class AmountModifier
{
    public ModifierType modifierType;

    public int amount;

    public AmountModifier(ModifierType modifierType, int amount)
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
                returnInt = currentPoints + amount;
                break;
            case ModifierType.Subtraction:
                returnInt = currentPoints - amount;
                break;
            case ModifierType.Mult:
                returnInt = currentPoints * amount;
                break;
            case ModifierType.Division:
                returnInt = Tools.DivideAndRoundUp(currentPoints, amount);
                break;
            case ModifierType.Replace:
                returnInt = amount;
                break;

        }

        return returnInt;
    }
}

public enum ModifierType
{
    Mult,
    Division,
    Addition,
    Subtraction,
    Replace
}
