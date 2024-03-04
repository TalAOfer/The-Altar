using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/FormationRegistry")]
public class EnemyFormationRegistry : ScriptableObject
{
    public List<EnemyFormation> Formations;
}
