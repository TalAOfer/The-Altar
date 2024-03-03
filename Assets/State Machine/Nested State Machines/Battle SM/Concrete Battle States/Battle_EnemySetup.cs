using System.Collections;
using System.Collections.Generic;

public class Battle_EnemySetup : BaseBattleState
{
    public Battle_EnemySetup(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        SpawnEnemies();
        yield return null;
    }

    public void SpawnEnemies()
    {
        List<EnemySpawn> enemies = GetEnemies();

        for (int i = 0; i < enemies.Count; i++)
        {
            CardBlueprint cardBlueprint = enemies[i].Blueprint;
            int gridIndex = (int)enemies[i].Placement;

            Card enemy = _ctx.EnemyCardManager.SpawnEnemyInIndexByBlueprint(gridIndex, cardBlueprint);
            _ctx.EnemyCardManager.ActiveEnemies.Add(enemy);
        }
    }
    private List<EnemySpawn> GetEnemies()
    {
        List<EnemySpawn> enemies = new();


        for (int row = 0; row < _ctx.BattleBlueprint.cardGrid.GetLength(0); row++)
        {
            for (int col = 0; col < _ctx.BattleBlueprint.cardGrid.GetLength(1); col++)
            {
                if (_ctx.BattleBlueprint.cardGrid[row, col] != null)
                {
                    GridPlacement_3 placement = GetGridPlacementFromIndices(row, col);
                    enemies.Add(new EnemySpawn(_ctx.BattleBlueprint.cardGrid[row, col], placement));
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