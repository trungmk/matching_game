using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GameStateType : byte
{
    HandleUserInput = 0,
    GenerateNewTile,
    Reload,
    Initialization,
    MatchingAllBoard
}

public class GameStateMachineMono : MonoBehaviour
{
    [SerializeField]
    private BoardController _boardController;

    [SerializeField]
    private float _stateTransitionTime = 0.2f;

    private IState _currentState;
    private GameStateType _currentStateType;

    // Game States
    private GenerateNewTileState _generateNewTileState;
    private HandleUserInputState _handleUserInputState;
    private ReloadState _reloadState;
    private InitializationState _initializationState;
    private MatchingAllBoardState _matchingAllBoardState;

    public BoardController BoardController => _boardController;

    private void Awake()
    {
        _generateNewTileState = new GenerateNewTileState(this);
        _handleUserInputState = new HandleUserInputState(this);
        _reloadState = new ReloadState(this);
        _initializationState = new InitializationState(this);
        _matchingAllBoardState = new MatchingAllBoardState(this);
    }

    public void Initialize()
    {
        //if (isLocalData)
        //{
        //    // Set the initial state
        //    _currentStateType = GameStateType.HandleUserInput;
        //    TransitionTo(_handleUserInputState);
        //}
        //else
        //{
        //    _currentStateType = GameStateType.Initialization;
        //    TransitionTo(_initializationState);
        //}

        _currentStateType = GameStateType.Initialization;
        TransitionTo(_initializationState);
    }

    public void TransitionTo(IState newState)
    {
        if (newState == null)
        {
            Debug.LogError("Cannot transition to null state!");
            return;
        }

        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = newState;
        _currentState.Enter();
    }

    public void TransitionToState(GameStateType stateType)
    {
        IState targetState = null;

        switch (stateType)
        {
            case GameStateType.HandleUserInput:
                targetState = _handleUserInputState;
                break;

            case GameStateType.GenerateNewTile:
                targetState = _generateNewTileState;
                break;

            case GameStateType.Reload:
                targetState = _reloadState;
                break;

            case GameStateType.Initialization:
                targetState = _initializationState;
                break;

            case GameStateType.MatchingAllBoard:
                targetState = _matchingAllBoardState;
                break;
        }

        if (targetState != null)
        {
            _currentStateType = stateType;
            TransitionTo(targetState);
        }

        Tilemap tilemap = null;
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Update();
        }
    }
}