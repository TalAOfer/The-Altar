using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName ="Registries/Sprite")]
public class SpriteRegistry : ScriptableObject
{
    public List<Sprite> sprites = new();
}
