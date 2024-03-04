using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Locator : MonoBehaviour
{
    [SerializeField] private PrefabRegistry _prefabs;
    public static PrefabRegistry Prefabs { get; private set; }

    [SerializeField] private EventRegistry _events;
    public static EventRegistry Events { get; private set; }

    [SerializeField] private RunData _runData;
    public static RunData RunData { get; private set; }

    [SerializeField] private FloorData _floorData;
    public static FloorData FloorData { get; private set; }
    public static PlayerActionProvider PlayerActionProvider { get; private set; }


    private void Awake()
    {
        Events = _events;
        RunData = _runData;
        FloorData = _floorData;
        Prefabs = _prefabs;

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
