using UnityEngine;

public class OpenLinkScript : MonoBehaviour
{

    [SerializeField] string url = "http://www.example.com";

    public void OpenURL()
    {
        Application.OpenURL(url);
    }

}
