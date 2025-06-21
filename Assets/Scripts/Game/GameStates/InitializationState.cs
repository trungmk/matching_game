using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InitializationState : IState
{
    private GameStateMachineMono _gameStateMachine;
    public InitializationState(GameStateMachineMono gameStateMachine)
    {
        _gameStateMachine = gameStateMachine;
    }
    public void Enter()
    {
        BoardData boardData = GameDataManager.Instance.CurrentBoardData;
        _gameStateMachine.BoardController.InitializeBoard(boardData);

        MatchSystem.CheckMatch(Vector2Int.zero, _gameStateMachine.BoardController);
        // Transition to the first state
        //_gameStateMachine.TransitionTo(_gameStateMachine.HandleUserInputState);
    }
    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }
}
