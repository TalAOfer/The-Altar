using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Floor")]
public class FloorData : ScriptableObject
{
    public int currentRoomIndex;
    public Codex enemyCodex;
}
