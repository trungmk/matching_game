using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HandleUserInputState : IState
{
    private readonly GameStateMachineMono _stateMachine;

    private BaseTile _firstTile;
    private BaseTile _secondTile;
    private Camera _mainCamera;

    public HandleUserInputState(GameStateMachineMono stateMachine)
    {
        _stateMachine = stateMachine;
        _mainCamera = Camera.main;
    }

    public void Enter()
    {
        EventManager.Instance.AddListener<TouchDownEvent>(Handle_TouchDownEvent);
        //EventManager.Instance.AddListener<DragEvent>(Handle_DragEvent);
        EventManager.Instance.AddListener<TouchUpEvent>(Handle_TouchUpEvent);
    }

    public void Exit()
    {
        EventManager.Instance.RemoveListener<TouchDownEvent>(Handle_TouchDownEvent);
        //EventManager.Instance.RemoveListener<DragEvent>(Handle_DragEvent);
        EventManager.Instance.RemoveListener<TouchUpEvent>(Handle_TouchUpEvent);
    }

    public void Update()
    {
        
    }

    private void Handle_TouchUpEvent(TouchUpEvent @event)
    {
        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(@event.TouchedPosition);
        BaseTile tile = _stateMachine.BoardController.GetTileFromWorldPosition(worldPos);

        if (tile != null && tile is Tile)
        {
            _secondTile = tile;

            if (_firstTile != null && _secondTile != null)
            {
                if (IsAdjacent(_firstTile.BoardPosition, _secondTile.BoardPosition))
                {
                    _stateMachine.BoardController.SwapTiles(_firstTile, _secondTile);
                    _stateMachine.BoardController.PositionChangedList.Add(_firstTile.BoardPosition);
                    _stateMachine.BoardController.PositionChangedList.Add(_secondTile.BoardPosition);
                    _stateMachine.TransitionToState(StateType.Matching);
                }

                // Reset the selected tiles
                _firstTile = null;
                _secondTile = null;
            }
            else
            {
                Debug.LogWarning("One or both of the selected tiles are null.");
            }
        }
        else
        {
            Debug.LogWarning("No tile found at the touched position.");
        }


    }

    private void Handle_TouchDownEvent(TouchDownEvent @event)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(@event.TouchedPosition);
        BaseTile tile = _stateMachine.BoardController.GetTileFromWorldPosition(worldPos);

        if (tile != null && tile is Tile)
        {
            _firstTile = tile;
        }
        else
        {
            Debug.LogWarning("No tile found at the touched position.");
            return;
        }
    }

    private void Handle_DragEvent(DragEvent @event)
    {
        
    }

    private bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        // Calculate Manhattan distance between tiles
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        bool isAdjacent = (dx + dy == 1);
        return isAdjacent;
    }
}