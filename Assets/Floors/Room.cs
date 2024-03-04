using Sirenix.OdinInspector;
using System;

[Serializable]
public class Room
{
    public RoomType Type;
    public RewardType Reward;
    [ShowIf("Type", RoomType.Battle)]
    public BattleBlueprint BattleBlueprint;
    [ShowIf("Reward", RewardType.Cards)]
    public int AmountOfOptions;

    public Room(RoomType type, RewardType reward)
    {
        Type = type;
        Reward = reward;
    }
    public void InitializeRoom(int index, int total, BattleRoomPool battlePool = null)
    {
        switch (Type)
        {
            case RoomType.Nothing:
                break;
            case RoomType.Battle:
                BattleBlueprint = battlePool.GetBattleBlueprintAccordingToIndex(index, total);
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


