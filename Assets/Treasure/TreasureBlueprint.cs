using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Treasure")]
public class TreasureBlueprint : ScriptableObject
{
    public List<TreasureItemBlueprint> Items;
}
public enum TreasureItemType
{
    Money,
    Pack
}


[Serializable]
public class TreasureItemBlueprint
{
    public TreasureItemType ItemType;
    public Vector2Int MinMaxAmount;

    public TreasureItem InstantiateItem()
    {
        int amount = UnityEngine.Random.Range(MinMaxAmount.x, MinMaxAmount.y + 1);
        return new TreasureItem(ItemType, amount);
    }
}