using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Palette")]
public class Palette : ScriptableObject
{
    public Color white;
    public Color darkPurple;
    public Color darkRed;
    public Color lightRed;
    public Color attackable;
    public Color strong;

    public Color GetColorByEnum(PaletteColor colorEnum)
    {
        switch (colorEnum)
        {
            case PaletteColor.white:
                return white;
            case PaletteColor.darkPurple:
                return darkPurple;
            case PaletteColor.darkRed:
                return darkRed;
            case PaletteColor.lightRed:
                return lightRed;
            case PaletteColor.attackable:
                return attackable;
            case PaletteColor.strong:
                return strong;
            default:
                return Color.green;
        }

    }
}

public enum PaletteColor
{
    white,
    darkPurple,
    darkRed,
    lightRed,
    attackable,
    strong
}
