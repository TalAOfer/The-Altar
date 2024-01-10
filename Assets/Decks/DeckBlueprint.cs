using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Deck")]
public class DeckBlueprint: ScriptableObject
{
    public List<CardBlueprint> cards;
}