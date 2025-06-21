using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum StateType : byte
{
    HandleUserInput = 0,
    Matching,
    GenerateNewTile,
    Reload,
    Initialization
}

public class GameStateMachineMono : MonoBehaviour
{
    [SerializeField]
    private BoardController _boardController;

    [SerializeField]
    private float _stateTransitionTime = 0.2f;

    private IState _currentState;
    private StateType _currentStateType;
    private MatchSystem _matchSystem;

    // Game States
    private GenerateNewTileState _generateNewTileState;
    private HandleUserInputState _handleUserInputState;
    private MatchingState _matchingState;
    private ReloadState _reloadState;
    private InitializationState _initializationState;

    public BoardController BoardController => _boardController;

    private void Awake()
    {
        _generateNewTileState = new GenerateNewTileState(this);
        _handleUserInputState = new HandleUserInputState(this);
        _matchingState = new MatchingState(this);
        _reloadState = new ReloadState(this);
        _initializationState = new InitializationState(this);
    }

    public void Initialize(bool isLocalData)
    {
        if (isLocalData)
        {
            // Set the initial state
            _currentStateType = StateType.HandleUserInput;
            TransitionTo(_handleUserInputState);
        }
        else
        {
            _currentStateType = StateType.Initialization;
            TransitionTo(_initializationState);
        }
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

    public void TransitionToState(StateType stateType)
    {
        IState targetState = null;

        switch (stateType)
        {
            case StateType.HandleUserInput:
                targetState = _handleUserInputState;
                break;

            case StateType.Matching:
                targetState = _matchingState;
                break;

            case StateType.GenerateNewTile:
                targetState = _generateNewTileState;
                break;

            case StateType.Reload:
                targetState = _reloadState;
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