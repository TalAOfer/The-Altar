using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="Floor")]
public class Floor : ScriptableObject
{
    [TableList(ShowIndexLabels = true)]
    public List<RoomBlueprint> rooms;
}
