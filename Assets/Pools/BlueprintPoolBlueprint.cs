using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprint Pool")]
public class BlueprintPoolBlueprint : ScriptableObject
{
    public List<CardBlueprint> black;
    public List<CardBlueprint> red;
}
