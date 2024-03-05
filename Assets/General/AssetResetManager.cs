#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AssetResetManager
{
    static AssetResetManager()
    {
        EditorApplication.playModeStateChanged += HandleAssetReset;
    }

    private static void HandleAssetReset(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange == PlayModeStateChange.ExitingPlayMode)
        {
            ResetAllIResetOnPlayModeExitAssets();
        }
    }

    private static void ResetAllIResetOnPlayModeExitAssets()
    {
        string[] resetAssetGUIDs = AssetDatabase.FindAssets("l:architecture");

        if (resetAssetGUIDs.Length == 0)
        {
            return;
        }

        int resetCount = 0;
        foreach (var assetGUID in resetAssetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

            if (asset is IResetOnPlaymodeExit resettableAsset)
            {
                resettableAsset.PlaymodeExitReset();
                resetCount++;
            }
        }

        Debug.Log($"Asset Reset Manager: Reset {resetCount} assets after exiting play mode.");
    }
}
#endif

public interface IResetOnPlaymodeExit
{
#if UNITY_EDITOR
    public void PlaymodeExitReset();
#endif
}