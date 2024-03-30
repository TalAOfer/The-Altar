using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using Sirenix.OdinInspector.Editor.GettingStarted;

[CreateAssetMenu(menuName = "Blueprints/Rooms/Room Pool")]
public class BattleRoomPoolAsset : SerializedScriptableObject
{
    public List<BattleBlueprintAsset> Blueprints;
}


[Serializable]
public class BattleRoomPool
{
    public List<BattleBlueprint> Lists;

    public BattleRoomPool(BattleRoomPoolAsset poolBlueprint)
    {
        Lists = poolBlueprint.Blueprints.Select(blueprintAsset => blueprintAsset.Value).ToList();
    }
    public BattleBlueprint GetBattleBlueprintAccordingToIndex(int difficulty)
    {
        // Filter the list by difficulty
        List<BattleBlueprint> filteredList = Lists.Where(asset => asset.Difficulty == difficulty).ToList();

        // If there are no blueprints at the given difficulty, return null or handle accordingly
        if (!filteredList.Any())
        {
            return null;
        }

        // Select a random index from the filtered list
        int blueprintIndex = UnityEngine.Random.Range(0, filteredList.Count);

        // Get the selected BattleBlueprintAsset
        BattleBlueprint selectedAsset = filteredList[blueprintIndex];

        // Remove the selected asset from the original list to prevent re-selection
        Lists.Remove(selectedAsset);

        // Assuming BattleBlueprintAsset has a property Value of type BattleBlueprint
        return selectedAsset;
    }
}
