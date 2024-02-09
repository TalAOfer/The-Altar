using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinRoom : Room
{
    [SerializeField] private TextMeshProUGUI hightscoreTextUGUI;
    [TextArea(0,2)]
    [SerializeField] private string highscoreText;
    [TextArea(0, 2)]
    [SerializeField] private string beatHighscoreText;
    [SerializeField] private RoomData roomData;
    [SerializeField] private AllEvents events;

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
        
        foreach(Card card in roomData.PlayerManager.activeCards)
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
