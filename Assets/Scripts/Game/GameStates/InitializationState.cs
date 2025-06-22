using Cysharp.Threading.Tasks;
using UnityEngine;

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
        await UniTask.Delay(600);

        _gameStateMachine.TransitionToState(GameStateType.MatchingAllBoard);

    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }
}
