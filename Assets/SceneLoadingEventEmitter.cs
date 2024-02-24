using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingEventEmitter : MonoBehaviour
{

    public void ChangeScene(int sceneIndex)
    {
        Locator.Events.LoadScene.Raise(this, sceneIndex);
    }
}
