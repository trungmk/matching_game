using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

public class MatchingAllBoardState : IState
{
    private readonly GameStateMachineMono _gameStateMachine;

    public MatchingAllBoardState(GameStateMachineMono gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }

    public void Enter()
    {
        _gameStateMachine.BoardController.MatchedChains.Clear();
        List<HashSet<Tile>> matchedTileGroup = MatchSystem.CheckMatchWholeBoard(_gameStateMachine.BoardController);

        if (matchedTileGroup.Count > 0)
        {
            _gameStateMachine.BoardController.MatchedChains.AddRange(matchedTileGroup);
            MatchAllBoardAsync().Forget();
        }
        else
        {
            _gameStateMachine.TransitionToState(GameStateType.HandleUserInput);
        }

        //Timing.RunCoroutine(MatchTiles());
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }

    private async UniTask MatchAllBoardAsync()
    {
        BoardController boardController = _gameStateMachine.BoardController;

        List<HashSet<Tile>> matchedTileGroup = boardController.MatchedChains;
        var disappearTasks = new List<UniTask>();
        for (int i = 0; i < matchedTileGroup.Count; i++)
        {
            HashSet<Tile> matchedTiles = matchedTileGroup[i];
            foreach (Tile tile in matchedTiles)
            {
                disappearTasks.Add(tile.PlayDisappearFX());
                boardController.Board.Cells[tile.BoardPosition.x, tile.BoardPosition.y].Tile = null;
            }
        }

        await UniTask.WhenAll(disappearTasks);

        await _gameStateMachine.BoardController.ApplyGravity();

        await UniTask.Delay(500);

        _gameStateMachine.TransitionToState(GameStateType.GenerateNewTile);
    }
}