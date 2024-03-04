using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Floor")]
public class FloorData : ScriptableObject, IResetOnPlaymodeExit
{
    public int currentRoomIndex;
    public Codex enemyCodex;
    public PlayerCardManager playerManager;

    public void Initialize(CodexBlueprint enemyCodexBlueprint)
    {
        currentRoomIndex = 0;
        enemyCodex = new Codex(enemyCodexBlueprint);
    }

    public void PlaymodeExitReset()
    {
        currentRoomIndex = 0;
        enemyCodex = null;
        playerManager = null;
    }
}
