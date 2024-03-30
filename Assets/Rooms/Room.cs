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
    public void InitializeRoom(BattleRoomPool battlePool = null)
    {
        switch (Type)
        {
            case RoomType.Nothing:
                break;
            case RoomType.Battle:
                BattleBlueprint = battlePool.GetBattleBlueprintAccordingToIndex(Difficulty);
                break;
            case RoomType.Elite:
                break;
            case RoomType.Shop:
                break;
            case RoomType.Treasure:
                break;
            case RoomType.First:
                break;
            case RoomType.Boss:
                break;
        }
    }


}


