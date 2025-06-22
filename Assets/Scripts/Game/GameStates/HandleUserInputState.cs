using Core;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class HandleUserInputState : IState
{
    private readonly GameStateMachineMono _stateMachine;

    private BaseTile _firstTile;
    private BaseTile _secondTile;
    private Camera _mainCamera;

    private HashSet<Tile> _potentailMatchTiles;
    private CoroutineHandle _potentialMatchCoroutine;

    public HandleUserInputState(GameStateMachineMono stateMachine)
    {
        _stateMachine = stateMachine;
        _mainCamera = Camera.main;
    }

    public void Enter()
    {
        _potentailMatchTiles = null;
        _potentialMatchCoroutine = default;
        EventManager.Instance.AddListener<TouchDownEvent>(Handle_TouchDownEvent);
        //EventManager.Instance.AddListener<DragEvent>(Handle_DragEvent);
        EventManager.Instance.AddListener<TouchUpEvent>(Handle_TouchUpEvent);

        FindPotentialMatches();
    }

    public void Exit()
    {
        EventManager.Instance.RemoveListener<TouchDownEvent>(Handle_TouchDownEvent);
        //EventManager.Instance.RemoveListener<DragEvent>(Handle_DragEvent);
        EventManager.Instance.RemoveListener<TouchUpEvent>(Handle_TouchUpEvent);

        ResetAnimationPotentialTiles();
    }

    public void Update()
    {
    }

    private void FindPotentialMatches()
    {
        ResetAnimationPotentialTiles(); 

        _potentailMatchTiles = MatchSystem.FindPotentialMatchesForTile(_stateMachine.BoardController);
        if (_potentailMatchTiles == null)
        {
            Debug.Log("GAME OVER!");
            _stateMachine.TransitionToState(GameStateType.Reload);
            return;
        }

        if (_potentailMatchTiles.Count >= 3)
        {
            _potentialMatchCoroutine = Timing.RunCoroutine(ShowPotentialMatchAfterDelay());
        }
    }

    private void Handle_TouchUpEvent(TouchUpEvent @event)
    {
        ResetAnimationPotentialTiles();

        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(@event.TouchedPosition);
        Tile tile = _stateMachine.BoardController.GetTileFromWorldPosition<Tile>(worldPos);

        if (tile == null)
        {
            Debug.LogWarning("Second Tile not found at the touched position.");
            return;
        }

        _secondTile = tile;

        if (_firstTile != null && _secondTile != null)
        {
            if (!IsAdjacent(_firstTile.BoardPosition, _secondTile.BoardPosition))
            {
                return;
            }

            Timing.RunCoroutine(Swap());
        }
        else
        {
            Debug.LogWarning("One or both of the selected tiles are null.");
        }
    }

    private void Handle_TouchDownEvent(TouchDownEvent @event)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(@event.TouchedPosition);
        Tile tile = _stateMachine.BoardController.GetTileFromWorldPosition<Tile>(worldPos);

        if (tile != null)
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

    private void ResetAnimationPotentialTiles()
    {
        if (_potentialMatchCoroutine != default)
        {
            Timing.KillCoroutines(_potentialMatchCoroutine); 
            _potentialMatchCoroutine = default;
        }

        if (_potentailMatchTiles != null)
        {
            foreach (var tile in _potentailMatchTiles)
            {
                tile.transform.DOKill(); 
                tile.ResetToOriginalScale(); 
            }

            _potentailMatchTiles.Clear();
        }
    }

    private IEnumerator<float> Swap()
    {
        _stateMachine.BoardController.SwapTiles(_firstTile, _secondTile);

        yield return Timing.WaitForSeconds(0.15f);

        HashSet<Tile> matchedTiles = MatchSystem.CheckMatchAtPosition(_firstTile as Tile, _stateMachine.BoardController);
        if (matchedTiles.Count >= 3)
        {
            _stateMachine.TransitionToState(GameStateType.MatchingAllBoard);
            yield break;
        }

        matchedTiles = MatchSystem.CheckMatchAtPosition(_secondTile as Tile, _stateMachine.BoardController);
        if (matchedTiles.Count >= 3)
        {
            _stateMachine.TransitionToState(GameStateType.MatchingAllBoard);
            yield break;
        }

        // Revert
        _stateMachine.BoardController.SwapTiles(_secondTile, _firstTile);

        yield return Timing.WaitForOneFrame;

        _secondTile = null;
        _firstTile = null;
        FindPotentialMatches();
    }

    private bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        // Calculate Manhattan distance between tiles
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        bool isAdjacent = (dx + dy == 1);
        return isAdjacent;
    }

    private IEnumerator<float> ShowPotentialMatchAfterDelay()
    {
        yield return Timing.WaitForSeconds(3f);

        foreach (var tile in _potentailMatchTiles)
        {
            tile.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack).SetLoops(-1);
        }
    }
}