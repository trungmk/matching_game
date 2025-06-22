using Core;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNewTileState : IState
{
    private readonly GameStateMachineMono _stateMachine;

    public GenerateNewTileState(GameStateMachineMono stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        GenerateTilesAsync().Forget();
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }

    private async UniTask GenerateTilesAsync()
    {
        BoardController boardController = _stateMachine.BoardController;
        int boardSize = boardController.Board.BoardSize;
        var moveTasks = new List<UniTask>();

        for (int x = 0; x < boardSize; x++)
        {
            List<BoardCell> emptyCells = new List<BoardCell>();
            for (int y = boardSize - 1; y >= 0; y--)
            {
                BoardCell cell = boardController.Board.Cells[x, y];

                // Check for blockers, if found, stop checking further down this column
                if (cell.Tile != null && cell.Tile is Blocker)
                {
                    break;
                }

                if (cell.Tile == null)
                {
                    emptyCells.Add(cell);
                }
            }

            if (emptyCells.Count > 0)
            {
                for (int i = 0; i < emptyCells.Count; i++)
                {
                    BoardCell cell = emptyCells[i];
                    Tile newTile = await boardController.CreateRandomTile();
                    if (newTile == null) 
                        continue;

                    newTile.BoardPosition = new Vector2Int(x, cell.BoardPosition.y);
                    newTile.transform.localPosition = new Vector2(cell.LocalPosition.x, 13f);
                    cell.Tile = newTile;
                    var moveTask = newTile.LocalMoveTo(cell.LocalPosition, 0.13f);
                    moveTasks.Add(moveTask);

                    await UniTask.Delay(10);
                }
            }
        }

        await UniTask.WhenAll(moveTasks);

        await UniTask.Delay(200);

        _stateMachine.TransitionToState(GameStateType.MatchingAllBoard);
    }
}