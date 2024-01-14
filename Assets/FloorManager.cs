using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;

    [SerializeField] private EnemyCardSpawner spawner;
    [SerializeField] private Floor floor;

    [SerializeField] private GameObject battleRoomPrefab;
    [SerializeField] private GameObject cardPickingRoomPrefab;

    [SerializeField] private Transform newRoomSpawnPos;
    [SerializeField] private Transform oldRoomSwipePos;
    
    [SerializeField] private int currentRoomIndex;
    private RoomBlueprint CurrentRoomBlueprint => floor.rooms[currentRoomIndex];

    private Room previousRoom;
    private Room currentRoom;

    [SerializeField] private float swipeDuration;

    public void NextRoom()
    {
        previousRoom = currentRoom;

        currentRoomIndex++;

        currentRoom = SpawnRoom();
        InitializeRoom(currentRoom);

        StartCoroutine(LerpPosition(currentRoom.transform, Vector3.zero, swipeDuration));
        StartCoroutine(LerpPosition(previousRoom.transform, oldRoomSwipePos.position, swipeDuration));
    }


    [Button]
    public void InitializeFloor()
    {
        currentRoomIndex = 0;
        currentRoom = SpawnRoom();
        InitializeRoom(currentRoom);

        StartCoroutine(LerpPosition(currentRoom.transform, Vector3.zero, swipeDuration));
    }

    public Room SpawnRoom()
    {
        GameObject roomPrefab = CurrentRoomBlueprint.roomType is RoomType.Battle ? battleRoomPrefab : cardPickingRoomPrefab;
        GameObject roomGo = Instantiate(roomPrefab, newRoomSpawnPos.position, Quaternion.identity, this.transform);
        Room room = roomGo.GetComponent<Room>();
        return room;
    }

    public void InitializeRoom(Room room)
    {
        switch (CurrentRoomBlueprint.roomType)
        {
            case RoomType.Battle:
                BattleRoom battleRoom = room as BattleRoom;
                battleRoom.InitializeRoom(this, spawner, CurrentRoomBlueprint.difficulty);
                battleRoom.SpawnEnemies();
                break;
            case RoomType.CardPicking:
                break;
        }
    }

    [Button]


    private IEnumerator LerpPosition(Transform targetTransform, Vector3 newPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = targetTransform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            targetTransform.position = Vector3.Lerp(startPosition, newPosition, t);
            yield return null;
        }

        // Ensure the final position is set to the target
        targetTransform.position = newPosition;

        events.OnNewRoom.Raise(this, currentRoom);
    }

}
