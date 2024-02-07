using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLogHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        ResetLog();
    }
    public void ResetLog()
    {
        logText.text = "";
    }
    public void AddLogEntry(Component sender, object data)
    {
        string newText = (string)data;
        logText.text += "- " + newText + "\n";
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        // Wait for end of frame so the content size is updated
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0f; // Scroll to bottom
    }
}
