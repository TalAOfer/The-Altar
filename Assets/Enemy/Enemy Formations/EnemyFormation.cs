using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Formation")]
public class EnemyFormation : ScriptableObject
{
    public int RowAmount = 2;
    public float RowSpacing = 1.5f;
    public float ColumnSpacing = 1.15f;
}
