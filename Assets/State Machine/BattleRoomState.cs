
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public abstract class BattleRoomState : BaseFloorState
{
    protected BattleBlueprint BattleBlueprint;
    protected List<Card> ActiveEnemies;
    protected EnemyCardSpawner EnemyCardSpawner;

    protected BattleRoomState(BattleBlueprint battleBlueprint, FloorStateMachine stateMachine) : base(stateMachine)
    {
        BattleBlueprint = battleBlueprint;
    }
}

public abstract class Battle_SetupState : BattleRoomState
{
    protected Battle_SetupState(BattleBlueprint battleBlueprint, FloorStateMachine stateMachine) : base(battleBlueprint, stateMachine)
    {
    }

    public override IEnumerator EnterState()
    {
        InitializeDeck();
        
        yield return null;
    }

    private void InitializeDeck()
    {
        _ctx.RunData.playerDeck = new Deck(_ctx.RunData.playerDeck.min, _ctx.RunData.playerDeck.max);
    }

    public void SpawnEnemies()
    {
        List<EnemySpawn> enemies = GetEnemies();

        for (int i = 0; i < enemies.Count; i++)
        {
            CardBlueprint cardBlueprint = enemies[i].Blueprint;
            int gridIndex = (int)enemies[i].Placement;

            Card enemy = EnemyCardSpawner.SpawnEnemyInIndexByBlueprint(gridIndex, cardBlueprint);
            ActiveEnemies.Add(enemy);
        }
    }
    private List<EnemySpawn> GetEnemies()
    {
        List<EnemySpawn> enemies = new();


        for (int row = 0; row < BattleBlueprint.cardGrid.GetLength(0); row++)
        {
            for (int col = 0; col < BattleBlueprint.cardGrid.GetLength(1); col++)
            {
                if (BattleBlueprint.cardGrid[row, col] != null)
                {
                    GridPlacement_3 placement = GetGridPlacementFromIndices(row, col);
                    enemies.Add(new EnemySpawn(BattleBlueprint.cardGrid[row, col], placement));
                }
            }
        }

        return enemies;
    }
    private GridPlacement_3 GetGridPlacementFromIndices(int row, int col)
    {
        // Assuming 3x3 grid, adjust if your grid size is different
        return (GridPlacement_3)(row * 3 + col);
    }


}