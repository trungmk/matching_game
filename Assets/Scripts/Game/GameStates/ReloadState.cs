using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Core;
using MEC;

public class ReloadState : IState
{
    private readonly GameStateMachineMono _stateMachine;

    public ReloadState(GameStateMachineMono stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public async void Enter()
    {
        GameDataManager.Instance.LocalData = await GeneratingTiles.GetGeneratedBoardData(_stateMachine.BoardController);
        GameDataManager.Instance.UpdateBoardData(GameDataManager.Instance.LocalData);

        int boardSize = _stateMachine.BoardController.Board.BoardSize;
        List<UniTask> disappearTasks = new List<UniTask>();

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                Tile tile = _stateMachine.BoardController.Board.GetTileByBoardPosition<Tile>(new UnityEngine.Vector2Int(x, y));
                if (tile != null)
                {
                    disappearTasks.Add(tile.PlayDisappearFX());
                }
                else
                {
                    Blocker blocker = _stateMachine.BoardController.Board.GetTileByBoardPosition<Blocker>(new UnityEngine.Vector2Int(x, y));
                    if (blocker != null)
                    {
                        disappearTasks.Add(blocker.Disappear());
                    }
                }
            }
        }

        await UniTask.WhenAll(disappearTasks);

        await _stateMachine.BoardController.InitBoard(GameDataManager.Instance.CurrentBoardData);

        await UniTask.Delay(800);

        _stateMachine.TransitionToState(GameStateType.MatchingAllBoard);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}