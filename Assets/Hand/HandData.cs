using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu (menuName = "Hand Data")]
public class HandData : ScriptableObject
{
    public float battleDrawbackY = 1f;
    public int maxCardAmount;
    public float baseSpacing = 1.5f;
    public float baseRotationAngle = 20f;
    public Vector2 yOffsetFactorMinMax = new(0.1f, 0.3f);
    public float selectStateHeightIncrease = 2f;
}
