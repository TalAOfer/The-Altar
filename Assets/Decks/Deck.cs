using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Deck")]
public class Deck: ScriptableObject
{
    public List<CardBlueprint> cards;
}