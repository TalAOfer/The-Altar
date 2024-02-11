using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingEventEmitter : MonoBehaviour
{
    private AllEvents events;

    private void Awake()
    {
        events = Tools.GetEvents();
    }

    public void ChangeScene(int sceneIndex)
    {
        events.LoadScene.Raise(this, sceneIndex);
    }
}
