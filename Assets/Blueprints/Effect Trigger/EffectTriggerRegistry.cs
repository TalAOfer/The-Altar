using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TriggerRegistry", menuName = "Effects/Trigger Registry")]
public class EffectTriggerRegistry : ScriptableObject
{
    public List<EffectTrigger> triggers;
#if UNITY_EDITOR

    [Button]
    public void FindAllTriggers()
    {
        triggers.Clear(); // Clear the list to prevent duplicates

        string[] guids = AssetDatabase.FindAssets("t:EffectTrigger");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            EffectTrigger trigger = AssetDatabase.LoadAssetAtPath<EffectTrigger>(assetPath);
            if (trigger != null)
            {
                triggers.Add(trigger);
                if (trigger.name != trigger.TriggerType.ToString())
                {
                    Debug.Log("Name - enum mismatch in " + trigger.name);
                }
            }
        }

        EditorUtility.SetDirty(this); // Mark the asset as dirty to ensure the changes are saved
        AssetDatabase.SaveAssets();
    }
#endif
    public EffectTrigger GetTriggerByEnum(TriggerType type)
    {
        foreach (EffectTrigger trigger in triggers)
        {
            if (trigger.TriggerType == type)
            {
                return trigger; // Found the matching trigger
            }
        }
        return null; // No matching trigger found
    }
}
