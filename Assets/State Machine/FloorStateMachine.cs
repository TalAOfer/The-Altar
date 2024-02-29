using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorStateMachine : MonoBehaviour
{
    private BaseFloorState _currentState;

    public RunData RunData { get; private set; }

    #region Floor Data

    [SerializeField] private Codex _enemyCodex;
    public Codex EnemyCodex { get { return _enemyCodex; } }

    #endregion

    #region Room Data 

    public List<MapSlot> grid;

    #endregion


    private void Awake()
    {
        RunData = Locator.RunData;
    }
    private void InitializeStateMachine(BaseFloorState state)
    {
        _currentState = state;

        StartCoroutine(_currentState.EnterState());
    }
    public IEnumerator SwitchState(BaseFloorState newState)
    {
        yield return StartCoroutine(_currentState.ExitState());

        _currentState = newState;

        StartCoroutine(_currentState.EnterState());
    }
}
