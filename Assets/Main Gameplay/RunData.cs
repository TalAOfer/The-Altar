using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Run Data")]
public class RunData : ScriptableObject
{
    public Deck playerDeck;
    public Codex playerCodex;
    public MetaPoolInstance playerPool;
    public PlayerManager playerManager;
}
