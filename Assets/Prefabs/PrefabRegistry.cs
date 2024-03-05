using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Registries/Prefabs")]
public class PrefabRegistry : ScriptableObject
{
    public GameObject Card;

    #region Rooms
    public GameObject BattleRoom;
    #endregion

}
