using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Registries/Prefabs")]
public class PrefabRegistry : ScriptableObject
{
    public GameObject Card;
    public GameObject Room;
    public GameObject Title;

    #region Treasure

    public GameObject TreasureChest;
    public GameObject BoosterPack;

    #endregion
}
