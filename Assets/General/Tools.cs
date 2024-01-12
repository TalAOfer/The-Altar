using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;
using UnityEngine.AI;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public static class Tools
{
    public static int MultAndRoundUp(float original, float mult)
    {
        float calcPoints = original * mult;
        return Mathf.CeilToInt(calcPoints);
    }
    public static bool didSucceed(float chance)
    {
        int rand = Random.Range(0, 100);
        return (chance > rand);
    }

    public static void ShuffleList<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public static List<int> DivideRandomly(int number, int minPart, int maxPart)
    {
        var parts = new List<int>();
        int remaining = number;

        while (remaining > 0)
        {
            int rangeMax = Mathf.Min(remaining, maxPart);
            int nextNumber = Random.Range(minPart, rangeMax + 1);
            parts.Add(nextNumber);
            remaining -= nextNumber;
        }

        return parts;
    }

    public static List<int> GetXUniqueRandoms(int count, int min, int max)
    {
        List<int> uniqueRandoms = new();
        while (uniqueRandoms.Count < count)
        {
            int randomNumber = Random.Range(min, max);
            if (!uniqueRandoms.Contains(randomNumber))
            {
                uniqueRandoms.Add(randomNumber);
            }
        }
        return uniqueRandoms;
    }
}
