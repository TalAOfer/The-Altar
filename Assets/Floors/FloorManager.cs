using DG.Tweening;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public RunData RunData { get; private set; }

    private PrefabRegistry _prefabs;

    private RoomStateMachine previousRoom;
    [SerializeField] private RoomStateMachine currentRoom;
    public int CurrentRoomIndex { get; private set; } = -1;
    private Vector3 _newRoomSpawnPosition = new(0, 20, 0);

    [SerializeField] private CodexBlueprint _enemyCodexBlueprint;
    public Codex EnemyCodex { get; private set; }

    //public FloorBlueprint floorBlueprint;
    [SerializeField] private FloorBlueprint _floorBlueprint;
    public Floor Floor { get; private set; }


    private void Awake()
    {
        RunData = Locator.RunData;
        RunData.Initialize();

        _prefabs = Locator.Prefabs;

        Floor = new Floor(_floorBlueprint);
        EnemyCodex = new Codex(_enemyCodexBlueprint);

        currentRoom.Initialize(this, Floor.FirstRoom);
        currentRoom.InitializeStateMachine();
    }

    private void OnDisable()
    {
        Pooler.ClearPools();
    }

    public void NextRoom(Room roomBlueprint)
    {
        CurrentRoomIndex++;

        RoomStateMachine newRoom = SpawnRoom();

        newRoom.Initialize(this, roomBlueprint);

        previousRoom = currentRoom;
        currentRoom = newRoom;

        SwipeRooms()
            .OnComplete(() =>
        {
            newRoom.InitializeStateMachine();
            Destroy(previousRoom.gameObject);
        });
    }

    private Sequence SwipeRooms()
    {
        Sequence swipeSequence = DOTween.Sequence();
        swipeSequence.Append(currentRoom.transform.DOLocalMoveY(0, 1).SetEase(Ease.InOutQuad));
        swipeSequence.Join(previousRoom.transform.DOLocalMoveY(-20, 1).SetEase(Ease.InOutQuad));

        return swipeSequence;
    }

    public RoomStateMachine SpawnRoom()
    {
        GameObject roomGo = Instantiate(_prefabs.Room, _newRoomSpawnPosition, Quaternion.identity, transform);
        RoomStateMachine room = roomGo.GetComponent<RoomStateMachine>();
        return room;
    }
}
