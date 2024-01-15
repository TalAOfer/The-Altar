using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;

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

    [Button]
    public void InitializeFloor()
    {
        StartCoroutine(FirstRoomRoutine());
    }


    private IEnumerator FirstRoomRoutine()
    {
        currentRoomIndex = 0;
        currentRoom = SpawnRoom();
        currentRoom.InitializeRoom(this, CurrentRoomBlueprint);

        yield return StartCoroutine(LerpPosition(currentRoom.transform, Vector3.zero, swipeDuration));
        currentRoom.OnRoomFinishedLerping();

        if (CurrentRoomBlueprint.roomType == RoomType.Battle)
        {
            events.OnNewRoom.Raise(this, CurrentRoomBlueprint);
        }
    }

    public void NextRoom()
    {
        StartCoroutine(NextRoomRoutine());
    }


    private IEnumerator NextRoomRoutine()
    {
        previousRoom = currentRoom;

        currentRoomIndex++;

        currentRoom = SpawnRoom();
        currentRoom.InitializeRoom(this, CurrentRoomBlueprint);

        StartCoroutine(LerpPosition(currentRoom.transform, Vector3.zero, swipeDuration));

        previousRoom.OnRoomFinished();
        yield return StartCoroutine(LerpPosition(previousRoom.transform, oldRoomSwipePos.position, swipeDuration));

        currentRoom.OnRoomFinishedLerping();

        if (CurrentRoomBlueprint.roomType == RoomType.Battle)
        {
            events.OnNewRoom.Raise(this, CurrentRoomBlueprint);
        }

        previousRoom.gameObject.SetActive(false);
    }

    public Room SpawnRoom()
    {
        GameObject roomPrefab = CurrentRoomBlueprint.roomType is RoomType.Battle ? battleRoomPrefab : cardPickingRoomPrefab;
        GameObject roomGo = Instantiate(roomPrefab, newRoomSpawnPos.position, Quaternion.identity, this.transform);
        Room room = roomGo.GetComponent<Room>();
        return room;
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
    }

}
