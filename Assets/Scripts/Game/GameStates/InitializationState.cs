using MEC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

public class InitializationState : IState
{
    private GameStateMachineMono _gameStateMachine;

    public InitializationState(GameStateMachineMono gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }
    public void Enter()
    {
        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        _gameStateMachine.BoardController.MatchedChains.Clear();
        var boardData = GameDataManager.Instance.CurrentBoardData;

        await _gameStateMachine.BoardController.InitializeBoard(boardData);
        await UniTask.Delay(1000);

        _gameStateMachine.TransitionToState(GameStateType.MatchingAllBoard);
    }

    //private IEnumerator<float> MatchTiles()
    //{
    //    yield return Timing.WaitForSeconds(1f);
    //    BoardController boardController = _gameStateMachine.BoardController;

    //    List<HashSet<Tile>> matchedTileGroup = boardController.MatchedChains;
    //    for (int i = 0; i < matchedTileGroup.Count; i++)
    //    {
    //        HashSet<Tile> matchedTiles = matchedTileGroup[i];
    //        foreach (Tile tile in matchedTiles)
    //        {
    //            tile.PlayDisappearFX();
    //            boardController.Board.Cells[tile.BoardPosition.x, tile.BoardPosition.y].Tile = null;
    //        }
    //    }

    //    yield return Timing.WaitForSeconds(1f);

    //    _gameStateMachine.BoardController.ApplyGravity();


    //    _gameStateMachine.TransitionToState(GameStateType.GenerateNewTile);
    //}

    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }
}
