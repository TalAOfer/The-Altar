using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(menuName ="Blueprints/Floor")]
public class FloorBlueprint : ScriptableObject
{
    public CodexBlueprint EnemyCodexBlueprint;
    public BattleRoomPoolAsset BattleRoomPool;

    public RoomBlueprint FirstRoom;
    [TableList(ShowIndexLabels = true)]
    public List<FloorLevel> Levels;
}
