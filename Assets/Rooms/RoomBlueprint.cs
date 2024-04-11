using Sirenix.OdinInspector;
using System;

[Serializable]
public class RoomBlueprint
{
    public RoomType Type;
    public RoomEvent RoomEvents; 
    [ShowIf("@RoomEvents.HasFlag(RoomEvent.Battle) && !PredetermineBattle")]
    public int Difficulty;

    [ShowIf("@RoomEvents.HasFlag(RoomEvent.Reward)")]
    public TreasureBlueprint Reward;

    [FoldoutGroup("Room Data")]
    [ShowIf("@RoomEvents.HasFlag(RoomEvent.Battle)")]
    public bool PredetermineBattle;

    [FoldoutGroup("Room Data")]
    [ShowIf("@PredetermineBattle && RoomEvents.HasFlag(RoomEvent.Battle)")]
    public PredeterminedBattleBlueprint BattleBlueprint;

    [FoldoutGroup("Room Data")]
    [ShowIf("@RoomEvents.HasFlag(RoomEvent.Battle)")]
    public bool PredetermineDeck;

    [FoldoutGroup("Room Data")]
    [ShowIf("@PredetermineDeck && RoomEvents.HasFlag(RoomEvent.Battle)")]
    public Deck Deck;

    public RoomBlueprint(RoomType type, TreasureBlueprint reward)
    {
        Type = type;
        Reward = reward;
    }
}

public enum RoomType
{
    Regular,
    Shop,
}

[Flags]
public enum RoomEvent
{
    Battle = 1,
    Reward = 2
}