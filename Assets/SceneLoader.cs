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
                gameObject.SetActive(false);
                OnFadeInEnd.Invoke(this, null);
            }
            );
        }

        else
        {
            gameObject.SetActive(false);
        }
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
