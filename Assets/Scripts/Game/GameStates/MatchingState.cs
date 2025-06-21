using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MatchingState : IState
{
    private readonly GameStateMachineMono _stateMachine;

    private MatchSystem _matchSystem;

    public MatchingState(GameStateMachineMono stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.BoardController.MatchedChains.Clear();
        List<Vector2Int> positionChangedList = _stateMachine.BoardController.PositionChangedList;
        if (positionChangedList.Count <= 0)
        {
            _stateMachine.TransitionToState(StateType.HandleUserInput);
            return;
        }

        for (int i = 0; i < positionChangedList.Count; i++)
        {
            List<Tile> matchedTiles = MatchSystem.CheckMatchAtPosition(positionChangedList[i], _stateMachine.BoardController);
            if (matchedTiles != null || matchedTiles.Count >= 3)
            {
                _stateMachine.BoardController.MatchedChains.Add(matchedTiles);
            }
        }

        Debug.Log($"Matched chains count: {_stateMachine.BoardController.MatchedChains.Count}");
    }

    private void CheckMatch(Vector2Int boardPos)
    {
        //_stateMachine.
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
    }
}