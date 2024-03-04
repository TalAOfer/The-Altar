using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Blueprints/Rooms/Room Pool")]
public class BattleRoomPoolAsset : SerializedScriptableObject
{
    public BattleRoomPool Value;
}

[Serializable]
public class BattleRoomPool
{
    public List<BattleBlueprint> Earlyfloor;
    public List<BattleBlueprint> Midfloor;
    public List<BattleBlueprint> Endfloor;

    public BattleRoomPool(BattleRoomPoolAsset blueprint)
    {
        Earlyfloor = new List<BattleBlueprint>(blueprint.Value.Earlyfloor);
        Midfloor = new List<BattleBlueprint>(blueprint.Value.Midfloor);
        Endfloor = new List<BattleBlueprint>(blueprint.Value.Endfloor);
    }

    public BattleBlueprint GetBattleBlueprintAccordingToIndex(int index, int totalRoomCount)
    {
        float ratio = (float)index / totalRoomCount;

        // Determine the list based on the ratio
        List<BattleBlueprint> selectedList;
        if (ratio <= 1f / 3) // One third or less, select from Earlyfloor
        {
            selectedList = Earlyfloor;
        }
        else if (ratio <= 2f / 3) // Two thirds or less, select from Midfloor
        {
            selectedList = Midfloor;
        }
        else // More than two thirds, select from Endfloor
        {
            selectedList = Endfloor;
        }

        // Ensure the selected list is not empty to avoid errors
        if (selectedList.Count == 0)
        {
            Debug.LogError("Selected list is empty.");
            return null; // Or handle this case as needed
        }

        // Select a random blueprint from the determined list
        return selectedList[UnityEngine.Random.Range(0, selectedList.Count)];
    }
}
