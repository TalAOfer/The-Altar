using Sirenix.OdinInspector;
using System;

[Serializable]
public class Room
{
    public RoomType Type;
    [ReadOnly]
    public BattleBlueprint BattleBlueprint;
    [ShowIf("Type", RoomType.Battle)]
    public int Difficulty;


    public RewardType Reward;
    [ShowIf("Reward", RewardType.Cards)]
    public int AmountOfOptions;

    public Room(RoomType type, RewardType reward)
    {
        Type = type;
        Reward = reward;
    }
}


