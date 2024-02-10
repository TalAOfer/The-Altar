using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public static class Tools
{
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera.main, out var result);
        return result;
    }

    public static void DeleteChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }

    public static int DivideAndRoundUp(int original, int divisor)
    {
        float calcPoints = (float)original / divisor;
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
            if (minPart > remaining)
            {
                parts.Add(remaining);
                break; // Exit the loop since remaining is less than the minimum part size.
            }

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
        HashSet<int> potentialNumbers = new();

        // Populate the set with all possible numbers in the range
        for (int i = min; i < max; i++)
        {
            potentialNumbers.Add(i);
        }

        // Loop until we have the required count or we run out of unique numbers
        while (uniqueRandoms.Count < count && potentialNumbers.Count > 0)
        {
            int randomNumber = Random.Range(min, max);

            // Only add if the number is still available
            if (potentialNumbers.Remove(randomNumber))
            {
                uniqueRandoms.Add(randomNumber);
            }
        }

        return uniqueRandoms;
    }

    public static string GetCardNameByArchetype(CardArchetype archetype, CardOwner owner)
    {
        string amountName = "";

        switch (archetype.points)
        {
            case 1:
                amountName = "One";
                break;
            case 2:
                amountName = "Two";
                break;
            case 3:
                amountName = "Three";
                break;
            case 4:
                amountName = "Four";
                break;
            case 5:
                amountName = "Five";
                break;
            case 6:
                amountName = "Six";
                break;
            case 7:
                amountName = "Seven";
                break;
            case 8:
                amountName = "Eight";
                break;
            case 9:
                amountName = "Nine";
                break;
            case 10:
                amountName = "Ten";
                break;
        }

        string symbolName = "";
        switch (owner)
        {
            case CardOwner.Player:
                symbolName = archetype.color == CardColor.Red ? "Hearts" : "Spades";
                break;
            case CardOwner.Enemy:
                symbolName = archetype.color == CardColor.Red ? "Diamonds" : "Clubs";
                break;
        }

        return amountName + " of " + symbolName;
    }

    public static void PlaySound(string soundName, Transform emitter)
    {
        string eventName = "event:/" + soundName;
        FMODUnity.RuntimeManager.PlayOneShot(eventName, emitter.position);
    }

    public static AllEvents GetEvents()
    {
        return Resources.Load<AllEvents>("AllEvents");
    }
}
