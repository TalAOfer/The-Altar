using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Run Data")]
public class RunData : ScriptableObject
{
    public DeckInstance playerDeck;

    [FoldoutGroup("Player")]
    public BlueprintPoolInstance playerCodex;
    [FoldoutGroup("Player")]
    public MetaPoolInstance playerPool;

    [FoldoutGroup("Enemy")]
    public MetaPoolInstance enemyPool;
    [FoldoutGroup("Enemy")]
    public BlueprintPoolInstance enemyCodex;

}
