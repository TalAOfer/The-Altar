using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    private void Awake()
    {
        Tools.PlaySound("Ambient", transform);
    }
}
