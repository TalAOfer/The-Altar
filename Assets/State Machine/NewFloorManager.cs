using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFloorManager : MonoBehaviour
{
    public RunData RunData { get; private set; }

    private PrefabRegistry _prefabs;

    private RoomManager previousRoom;
    [SerializeField] private RoomManager currentRoom;
    public RoomBlueprint NextRoomBlueprint {  get; private set; }

    public int CurrentRoomIndex { get; private set; } = -1;
    private Vector3 _newRoomSpawnPosition = new(0, 20, 0);

    [SerializeField] private Codex _enemyCodex;
    public Codex EnemyCodex { get { return _enemyCodex; } }


    private void Awake()
    {
        RunData = Locator.RunData;
        RunData.Initialize();

        _prefabs = Locator.Prefabs;
    }
    private void OnDisable()
    {
        Pooler.ClearPools();
    }

    public void NextRoom(RoomBlueprint roomBlueprint)
    {
        RoomManager newRoom = SpawnRoomByType(roomBlueprint.RoomType); 

        previousRoom = currentRoom;
        currentRoom = newRoom;

        CurrentRoomIndex++;
    }

    public void OnRoomInitialized()
    {
        SwipeRooms().OnComplete(() =>
        {
            currentRoom.OnRoomReady();
        });
    }

    private Sequence SwipeRooms()
    {
        Sequence swipeSequence = DOTween.Sequence();
        swipeSequence.Append(currentRoom.transform.DOLocalMoveY(0, 1).SetEase(Ease.InOutQuad));
        swipeSequence.Append(previousRoom.transform.DOLocalMoveY(-20, 1).SetEase(Ease.InOutQuad));

        return swipeSequence;
    }

    public RoomManager SpawnRoomByType(RoomType roomType)
    {
        GameObject roomPrefab = null;

        switch (roomType)
        {
            case RoomType.Battle:
                roomPrefab = _prefabs.BattleRoom;
                break;
        }

        GameObject roomGo = Instantiate(roomPrefab, _newRoomSpawnPosition, Quaternion.identity, transform);
        RoomManager room = roomGo.GetComponent<RoomManager>();
        return room;
    }
}
