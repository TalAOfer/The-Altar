using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Locator : MonoBehaviour
{
    [SerializeField] private AllEvents _events;
    public static AllEvents Events { get; private set; }

    [SerializeField] private RunData _runData;
    public static RunData RunData { get; private set; }

    [SerializeField] private FloorData _floorData;
    public static FloorData FloorData { get; private set; }

    [SerializeField] private RoomData _roomData;
    public static RoomData RoomData { get; private set; }

    public static PlayerManager PlayerManager { get; private set; }

    public static DataProvider DataProvider {  get; private set; }
    public static PlayerActionProvider PlayerActionProvider { get; private set; }


    private void Awake()
    {
        Events = _events;
        RunData = _runData;
        FloorData = _floorData;
        RoomData = _roomData;

        PlayerManager = FindComponent<PlayerManager>();
        DataProvider = FindComponent<DataProvider>();
        PlayerActionProvider = FindComponent<PlayerActionProvider>();
    }

    private T FindComponent<T>() where T : Component
    {
        T component = FindObjectOfType<T>();
        if (component == null)
        {
            // Automatically generates an error message using the type's name
            Debug.LogError($"No {typeof(T).Name} in scene");
        }
        return component;
    }
}
