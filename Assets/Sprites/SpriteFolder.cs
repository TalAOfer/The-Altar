using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName ="SpriteFolder")]
public class SpriteFolder : ScriptableObject
{
    [Title("Symbols")]
    public Sprite clubs;
    public Sprite spades;
    public Sprite diamonds;
    public Sprite hearts;

    [Title("Numbers")]
    public List<Sprite> numbers;
}
