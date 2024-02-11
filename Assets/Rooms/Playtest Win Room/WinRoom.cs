using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinRoom : Room
{
    [SerializeField] private TextMeshProUGUI hightscoreTextUGUI;
    [TextArea(0,2)]
    [SerializeField] private string highscoreText;
    [TextArea(0, 2)]
    [SerializeField] private string beatHighscoreText;
    [SerializeField] private RoomData roomData;
    [SerializeField] private AllEvents events;
    [SerializeField] private Image mask;
    [SerializeField] private GameObject cardRain;

    public override void OnRoomFinishedLerping()
    {
        base.OnRoomFinishedLerping();
        events.EnableMask.Raise();
        cardRain.SetActive(true);
        mask.DOFade(0.25f, 1);
    }

    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        base.InitializeRoom(floorManager, roomBlueprint);
        InitializeText();
    }

    private void InitializeText()
    {

        string defaultText = GetScore() <= 8 ? highscoreText : beatHighscoreText;
        string score = GetScore().ToString();
        defaultText = string.Format(defaultText, score);
        hightscoreTextUGUI.text = defaultText;
    }

    private int GetScore()
    {
        int score = 0;
        
        foreach(Card card in roomData.PlayerManager.ActiveCards)
        {
            score += card.points;
        }

        return score;
    }
    
    public void SwitchScenes(int index)
    {
        events.LoadScene.Raise(this, index);
    }
}
