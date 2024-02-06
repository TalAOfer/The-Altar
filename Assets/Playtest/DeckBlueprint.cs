using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Blueprints/Deck")]
public class DeckBlueprint : ScriptableObject
{
    [Button]
    public void InitializeDeck()
    {
        deck = new DeckInstance(0, 10, false);
    } 

    public DeckInstance deck;

}
