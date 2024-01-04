using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Guardian
{
    public GuardianType guardianType;

    public Card guardianCard;

    public Guardian(GuardianType guardianType, Card guardianCard)
    {
        this.guardianType = guardianType;
        this.guardianCard = guardianCard;
    }

    public int ApplyAndGetRestOfDamage(int totalDamage, int currentCardPoints)
    {
        int returnInt = -1;
        switch (guardianType)
        {
            case GuardianType.Promise:
                int damageToPromise = Tools.MultAndRoundUp(totalDamage, 0.5f);
                int restOfDamage = totalDamage - damageToPromise;
                guardianCard.TakeDirectDamage(damageToPromise);
                returnInt = restOfDamage;
                break;
            case GuardianType.Significance:
                int finalHealth = 1; // The minimum health the main card can have
                int maxDamageMainCardCanTake = currentCardPoints - finalHealth; // Calculate the maximum damage the main card can take without dropping below final health
                // Calculate the actual damage to be dealt to the main card
                int damageToMainCard = Math.Min(totalDamage, maxDamageMainCardCanTake); // It's the smaller of the total incoming damage and the max damage main card can take
                // Ensure that the damage to main card is not negative
                if (damageToMainCard < 0) damageToMainCard = 0;
                // Calculate the damage to be absorbed by the guardian card
                int damageToSignificance = totalDamage - damageToMainCard;
                guardianCard.TakeDirectDamage(damageToSignificance);
                returnInt = damageToMainCard;
                break;
        }

        return returnInt;
    }
}

public enum GuardianType
{
    Promise,
    Significance
}