using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleAmountModifier
{
    public ModifierType ModifierType { get; private set; }
    public BattlePointType PointType { get; private set; }
    private readonly Card _parentCard;
    private readonly int _defaultAmount;
    private readonly BattleRoomDataProvider _data;
    private readonly GetAmountStrategy _amountStrategy;
    private readonly Decision _decision;
    public BattleAmountModifier(ModifierType modifierType, Card parentCard, BattlePointType pointType, int defaultAmount, GetAmountStrategy amountStrategy, BattleRoomDataProvider data, Decision decision = null)
    {
        ModifierType = modifierType;
        PointType = pointType;
        _parentCard = parentCard;
        _defaultAmount = defaultAmount;
        _amountStrategy = amountStrategy;
        _decision = decision;
        _data = data;
    }
    public int Apply(int currentPoints, Card opponent = null)
    {
        if (_decision != null)
        {
            if (_decision.Decide(_parentCard, opponent) == false)
            {
                return currentPoints;
            }
        }

        int amount = GetAmount(_parentCard);

        int returnInt = -10;

        switch (ModifierType)
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

    public int GetAmount(Card target)
    {
        return _amountStrategy is GetAmountStrategy.Value ? _defaultAmount
                : _data.GetAmount(target, _amountStrategy);
    }

}
