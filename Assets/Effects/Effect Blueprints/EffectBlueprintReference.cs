using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectBlueprintReference
{
    public bool UseAsset;
    [HideIf("UseAsset")]
    public EffectBlueprint ManualValue;
    [ShowIf("UseAsset")]
    public EffectBlueprintAsset Asset;

    public EffectBlueprint Value
    {
        get { return UseAsset ? Asset.blueprint : ManualValue; }
    }
}
