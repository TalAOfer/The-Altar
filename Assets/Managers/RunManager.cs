using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public void Awake()
    {
            Locator.RunData.Initialize();
    }
}
