using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private Floor floor;
    public int currentRoomIndex;

    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;

    [FoldoutGroup("Visuals")]
    [SerializeField] private float swipeDuration;

    [FoldoutGroup("Prefabs")]
    [SerializeField] private GameObject battleRoomPrefab;
    [FoldoutGroup("Prefabs")]
    [SerializeField] private GameObject cardPickingRoomPrefab;

    [FoldoutGroup("Transforms")]
    [SerializeField] private Transform newRoomSpawnPos;
    [FoldoutGroup("Transforms")]
    [SerializeField] private Transform oldRoomSwipePos;

    private RoomBlueprint CurrentRoomBlueprint => floor.rooms[currentRoomIndex];

    private Room previousRoom;
    private Room currentRoom;


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

        yield return StartCoroutine(currentRoom.AnimateDown());

        currentRoom.OnRoomFinishedLerping();

        if (CurrentRoomBlueprint.roomType == RoomType.Battle)
        {
            events.OnNewRoom.Raise(this, CurrentRoomBlueprint);
        }
    }

    public IEnumerator WaitForAnimationEnd(float animationDuration)
    {
        yield return new WaitForSeconds(animationDuration);
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

        previousRoom.OnRoomFinished();

        StartCoroutine(previousRoom.AnimateDown());
        yield return StartCoroutine(currentRoom.AnimateDown());

        currentRoom.OnRoomFinishedLerping();

        if (CurrentRoomBlueprint.roomType == RoomType.Battle)
        {
            events.OnNewRoom.Raise(this, CurrentRoomBlueprint);
        }

        previousRoom.gameObject.SetActive(false);
        Destroy(previousRoom.gameObject);
    }

    public Room SpawnRoom()
    {
        GameObject roomPrefab = CurrentRoomBlueprint.roomType is RoomType.Battle ? battleRoomPrefab : cardPickingRoomPrefab;
        GameObject roomGo = Instantiate(roomPrefab, newRoomSpawnPos.position, Quaternion.identity, this.transform);
        Room room = roomGo.GetComponent<Room>();
        return room;
    }
}
