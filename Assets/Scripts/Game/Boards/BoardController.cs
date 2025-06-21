using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private BoardMono _board;

    private List<Vector2Int> _positionChangedList = new List<Vector2Int>();

    public List<Vector2Int> PositionChangedList { get { return _positionChangedList; } }

    public List<HashSet<Tile>> MatchedChains = new List<HashSet<Tile>>();

    public BoardMono Board
    {
        get { return _board; }
    }

    public async UniTask InitializeBoard(BoardData boardData)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is not assigned in the inspector.");
            return;
        }

        await _board.Initialize(boardData);
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

    public T GetTileFromWorldPosition<T>(Vector2 worldPosition) where T : BaseTile
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return null;
        }

        return _board.GetTileFromWorldPosition<T>(worldPosition);
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

    public T GetTileByBoardPosition<T>(Vector2Int boardPosition) where T : BaseTile
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return null;
        }

        return _board.GetTileByBoardPosition<T>(boardPosition);
    }

    public void GetCellByBoardPosition(Vector2Int boardPosition)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return;
        }

        _board.GetCellByBoardPosition(boardPosition);
    }

    public async UniTask<Tile> CreateRandomTile()
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return null;
        }

        return await _board.CreateRandomTile();
    }

    public void ApplyGravity()
    {
        for (int x = 0; x < _board.BoardSize; x++)
        {
            int fallToY = -1;

            for (int y = 0; y < _board.BoardSize; y++)
            {
                BoardCell cell = _board.Cells[x, y];

                if (cell.IsBlocker)
                {
                    fallToY = -1;
                    continue;
                }

                if (cell.Tile == null)
                {
                    if (fallToY == -1)
                    {
                        fallToY = y;
                    }
                }
                else
                {
                    if (fallToY != -1)
                    {
                        Tile tile = cell.Tile as Tile;
                        BoardCell targetCell = _board.Cells[x, fallToY];

                        cell.Tile = null;
                        targetCell.Tile = tile;

                        tile.BoardPosition = targetCell.BoardPosition;
                        tile.LocalMoveTo(targetCell.LocalPosition);

                        fallToY++;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_board == null || _board.Cells == null)
            return;

        Gizmos.color = Color.red;
        for (int x = 0; x < _board.BoardSize; x++)
        {
            for (int y = 0; y < _board.BoardSize; y++)
            {
                var cell = _board.Cells[x, y];
                if (cell == null) 
                    continue;

                if (cell.Tile == null)
                {

                    Vector3 worldPos = _board.GetWorldPositionFromLocalPosition(cell.LocalPosition);

                    Gizmos.DrawCube(worldPos, Vector3.one * 0.3f);
                }
            }
        }
    }
}

