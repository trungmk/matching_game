using Core;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private BoardMono _board;

    private List<Vector2Int> _positionChangedList = new List<Vector2Int>();

    public List<Vector2Int> PositionChangedList { get { return _positionChangedList; } }

    public readonly List<List<Tile>> MatchedChains = new List<List<Tile>>();

    public BoardMono Board
    {
        get { return _board; }
    }

    public void InitializeBoard(BoardData boardData)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is not assigned in the inspector.");
            return;
        }

        _board.Initialize(boardData);
    }

    public Vector2Int GetTilePositionFromWorldPosition(Vector2 worldPosition)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return Vector2Int.zero;
        }

        return _board.GetTilePositionFromWorldPosition(worldPosition);
    }

    public BaseTile GetTileFromWorldPosition(Vector2 worldPosition)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return null;
        }

        return _board.GetTileFromWorldPosition(worldPosition);
    }

    public void SwapTiles(BaseTile firstTile, BaseTile secondTile)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return;
        }

        _board.SwapTiles(firstTile, secondTile);
    }

    public BaseTile GetTileByBoardPosition(Vector2Int boardPosition)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return null;
        }
        return _board.GetTileByBoardPosition(boardPosition);
    }
}
