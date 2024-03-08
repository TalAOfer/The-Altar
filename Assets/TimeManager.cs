using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Button]
    public void UpdateTime(float time)
    {
        Time.timeScale = time;
    }
}
