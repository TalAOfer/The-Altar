
public class BattlePoint
{
    public int value;
    public BattlePointType type;

    public BattlePoint(int value, BattlePointType type)
    {
        this.value = value;
        this.type = type;
    }
}

public enum BattlePointType
{
    Attack,
    Hurt
}