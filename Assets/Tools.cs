using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;
using UnityEngine.AI;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public static class Tools
{

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
}
