using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Rooms/Room Pool")]
public class RoomPool : SerializedScriptableObject
{
    public Dictionary<int, List<RoomBlueprint>> RoomsByDifficulty;

    public List<RoomBlueprint> AllRooms;

    [Button]
    public void SortRoomsToByDifficulty()
    {
        foreach (RoomBlueprint room in AllRooms)
        {
            if (!RoomsByDifficulty.ContainsKey(room.difficulty))
            {
                RoomsByDifficulty[room.difficulty] = new List<RoomBlueprint>();
            }

            // Add the room to the appropriate list
            RoomsByDifficulty[room.difficulty].Add(room);
        }
    }
}
