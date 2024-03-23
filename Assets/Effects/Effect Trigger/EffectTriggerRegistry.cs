using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TriggerRegistry", menuName = "Effects/Trigger Registry")]
public class EffectTriggerRegistry : ScriptableObject
{
    public List<EffectTriggerAsset> triggers;
#if UNITY_EDITOR

    [Button]
    public void FindAllTriggers()
    {
        triggers.Clear(); // Clear the list to prevent duplicates

        string[] guids = AssetDatabase.FindAssets("t:EffectTriggerAsset");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            EffectTriggerAsset trigger = AssetDatabase.LoadAssetAtPath<EffectTriggerAsset>(assetPath);
            if (trigger != null)
            {
                triggers.Add(trigger);

                // Attempt to parse the asset name into the TriggerType enum
                if (Enum.TryParse<TriggerType>(trigger.name, out var parsedEnum))
                {
                    // If successfully parsed and the value is different, update the trigger
                    if (trigger.TriggerType != parsedEnum)
                    {
                        trigger.TriggerType = parsedEnum;
                        EditorUtility.SetDirty(trigger); // Mark the trigger asset as dirty to ensure the change is saved
                        Debug.Log($"Updated TriggerType of {trigger.name} to match its name.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Unable to parse name of {trigger.name} into a valid TriggerType enum value.");
                }
            }
        }

        EditorUtility.SetDirty(this); // Mark the asset as dirty to ensure the changes are saved
        AssetDatabase.SaveAssets();
    }
#endif
    public EffectTriggerAsset GetTriggerAssetByEnum(TriggerType type)
    {
        foreach (EffectTriggerAsset trigger in triggers)
        {
            if (trigger.TriggerType == type)
            {
                return trigger; // Found the matching trigger
            }
        }
        return null; // No matching trigger found
    }
}
