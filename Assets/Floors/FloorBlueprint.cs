using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="Blueprints/Floor")]
public class FloorBlueprint : ScriptableObject
{
    public CodexBlueprint EnemyCodexBlueprint;

    [InfoBox("$InfoBoxMessage")]
    public int TotalNormalRoomAmount;

    public int AmountOfBattleMoneyRooms;
    public int AmountOfBattleCardRooms;
    public int AmountOfEliteRooms;
    public int AmountOfTreasureRooms;
    public int AmountOfShopRooms;

    public string InfoBoxMessage
    {
        get
        {
            int remainingRooms = TotalNormalRoomAmount * 2 - (AmountOfBattleMoneyRooms + AmountOfBattleCardRooms + AmountOfTreasureRooms + AmountOfShopRooms + AmountOfEliteRooms);
            if (remainingRooms == 0)
            {
                return "Room count matches exactly!";
            }
            else if (remainingRooms > 0)
            {
                return $"Too few rooms: Missing {remainingRooms} room(s).";
            }
            else // remainingRooms < 0
            {
                return $"Too many rooms: Exceed by {-remainingRooms} room(s).";
            }
        }
    }
}
