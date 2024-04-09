using DG.Tweening;
using System.Collections;
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

        currentRoom.Initialize(Floor, 0, Floor.FirstRoom, EnemyCodex);
    }

    private void OnDisable()
    {
        Pooler.ClearPools();
    }

    public void OnLevelFadeoutEnd()
    {
        StartCoroutine(FloorStartRoutine());
    }

    public IEnumerator FloorStartRoutine()
    {
        yield return ShowTitle();
        currentRoom.InitializeStateMachine();
    }

    public void NextRoom(RoomBlueprint roomBlueprint)
    {
        CurrentRoomIndex++;

        RoomStateMachine newRoom = SpawnRoom();

        newRoom.Initialize(Floor, CurrentRoomIndex, roomBlueprint, EnemyCodex);

        previousRoom = currentRoom;
        currentRoom = newRoom;

        SwipeRooms()
            .OnComplete(() =>
        {
            newRoom.InitializeStateMachine();
            Destroy(previousRoom.gameObject);
        });
    }

    public void OnDoorClicked(Component sender, object data)
    {
        NextRoom(data as RoomBlueprint);
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

    public IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject TitleGO = Instantiate(_prefabs.Title, _prefabs.Title.transform.position, Quaternion.identity, transform);
        SpriteRenderer TitleSR = TitleGO.GetComponent<SpriteRenderer>();

        yield return TitleSR.transform.DOLocalMoveY(6.5f, 1).SetEase(Ease.InOutFlash);

        //Fade title
        yield return TitleSR.DOFade(0, 2).SetEase(Ease.InExpo).WaitForCompletion();
    }
}
