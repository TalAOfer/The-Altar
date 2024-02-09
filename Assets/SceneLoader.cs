using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image screen;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private bool fadeIn;
    [SerializeField] private bool fadeOut;
    [ShowIf("fadeIn")]
    [SerializeField] private CustomGameEvent OnFadeInEnd;

    private void Start()
    {
        if (fadeIn)
        {
            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, 1f);
            screen.DOFade(0, 1).OnComplete(() =>
            {
                raycaster.enabled = false;
                OnFadeInEnd.Invoke(this, null);
            }
            );
        }

        else
        {
            raycaster.enabled = false;
        }
    }

    public void LoadScene(Component sender, object data)
    {
        int index = (int)data;
        SwitchScenes(index);
    }

    public void SwitchScenes(int sceneIndex)
    {
        gameObject.SetActive(true);

        if (fadeOut)
        {
            screen.DOFade(1, 1).OnComplete(
                () => SceneManager.LoadSceneAsync(sceneIndex)
                );
        }
        else
        {
            SceneManager.LoadSceneAsync(sceneIndex);
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
