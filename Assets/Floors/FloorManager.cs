using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private Floor floor;
    private FloorData floorData;
    
    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;

    [FoldoutGroup("Visuals")]
    [SerializeField] private float swipeDuration;

    [FoldoutGroup("Prefabs")]
    [SerializeField] private GameObject battleRoomPrefab;
    [FoldoutGroup("Prefabs")]
    [SerializeField] private GameObject cardPickingRoomPrefab;
    [FoldoutGroup("Prefabs")]
    [SerializeField] private GameObject playTestCardGainPrefab;
    [FoldoutGroup("Prefabs")]
    [SerializeField] private GameObject playTestWinPrefab;
    [FoldoutGroup("Prefabs")]
    [SerializeField] private GameObject tutorialBattleRoomPrefab;

    [FoldoutGroup("Transforms")]
    [SerializeField] private Transform newRoomSpawnPos;
    [FoldoutGroup("Transforms")]
    [SerializeField] private Transform oldRoomSwipePos;

    private RoomBlueprint CurrentRoomBlueprint => floor.Rooms[floorData.currentRoomIndex];

    private Room previousRoom;
    private Room currentRoom;


    private void Awake()
    {
        floorData = Locator.FloorData;
        floorData.enemyCodex = new Codex(floor.EnemyCodexBlueprint);
        
    }

    [Button]
    public void InitializeFloor()
    {
        StartCoroutine(FirstRoomRoutine());
    }


    private IEnumerator FirstRoomRoutine()
    {
        floorData.currentRoomIndex = 0;
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
        yield return Tools.GetWait(animationDuration);
    }

    public void NextRoom()
    {
        StartCoroutine(NextRoomRoutine());
    }


    private IEnumerator NextRoomRoutine()
    {
        events.OnNewRoom.Raise(this, CurrentRoomBlueprint);
        previousRoom = currentRoom;

        floorData.currentRoomIndex++;

        currentRoom = SpawnRoom();
        currentRoom.InitializeRoom(this, CurrentRoomBlueprint);

        previousRoom.OnRoomFinished();

        StartCoroutine(previousRoom.AnimateDown());
        yield return StartCoroutine(currentRoom.AnimateDown());

        currentRoom.OnRoomFinishedLerping();

        previousRoom.gameObject.SetActive(false);
        Destroy(previousRoom.gameObject);
    }

    public Room SpawnRoom()
    {
        GameObject roomPrefab = null;

        switch (CurrentRoomBlueprint.roomType)
        {
            case RoomType.Battle:
                roomPrefab = !CurrentRoomBlueprint.isTutorial ? battleRoomPrefab : tutorialBattleRoomPrefab;
                break;
            case RoomType.CardPicking:
                roomPrefab = cardPickingRoomPrefab;
                break;
            case RoomType.PlaytestCardGain:
                roomPrefab = playTestCardGainPrefab;
                break;
            case RoomType.PlaytestWin:
                roomPrefab = playTestWinPrefab;
                break;
        }

        GameObject roomGo = Instantiate(roomPrefab, newRoomSpawnPos.position, Quaternion.identity, this.transform);
        Room room = roomGo.GetComponent<Room>();
        return room;
    }
}
