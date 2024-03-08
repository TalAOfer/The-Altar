using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public abstract class RoomStateMachine : MonoBehaviour
{
    protected IRoomState _currentState;
    
    [ShowInInspector, ReadOnly]
    private string _currentStateName => _currentState?.GetType().Name ?? "None";
    public bool IsSwitchingStates { get; private set; }

    [SerializeField] protected Door leftDoor;
    [SerializeField] protected Door rightDoor;
    public FloorManager FloorCtx { get; private set; }
    public EventRegistry Events { get; private set; }
    public virtual void Initialize(FloorManager floorCtx, Room room)
    {
        FloorCtx = floorCtx;
        Events = Locator.Events;
        FloorLevel nextLevel = floorCtx.Floor.Levels[floorCtx.CurrentRoomIndex + 1];
        leftDoor.Initialize(floorCtx, nextLevel.LeftRoom);
        rightDoor.Initialize(floorCtx, nextLevel.RightRoom);
    }

    public abstract void InitializeStateMachine();

    public void SwitchState(IRoomState newState)
    {
        StartCoroutine(SwitchStateRoutine(newState));
    }

    public IEnumerator SwitchStateRoutine(IRoomState newState)
    {
        IsSwitchingStates = true;

        if (_currentState != null)
        {
            yield return StartCoroutine(_currentState.ExitState());
        }

        _currentState = newState;

        yield return StartCoroutine(_currentState.EnterState());

        IsSwitchingStates = false;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public IEnumerator OpenDoors()
    {
        Coroutine LeftDoorRoutine = StartCoroutine(leftDoor.OpenDoorRoutine());
        Coroutine RightDoorRoutine = StartCoroutine(rightDoor.OpenDoorRoutine());

        yield return LeftDoorRoutine;
        yield return RightDoorRoutine;
    }

    public GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return Instantiate(prefab, position, rotation, parent);
    }
}
