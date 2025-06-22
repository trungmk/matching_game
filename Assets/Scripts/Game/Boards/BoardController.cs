using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
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

    private void Awake()
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is not assigned in the inspector.");
            return;
        }
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

    public UniTask ApplyGravity()
    {
        var tasks = new List<UniTask>();
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
                        var moveTask = tile.LocalMoveTo(targetCell.LocalPosition, 0.13f);
                        tasks.Add(moveTask);

                        fallToY++;
                    }
                }
            }
        }
        return UniTask.WhenAll(tasks);
    }

    public async UniTask InitBoard(BoardData boardData)
    {
        await _board.InitBoard(boardData.Items);
    }

    public async UniTask<Tile> CloneTile(Tile tile, bool isActive = false)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return null;
        }
        if (tile == null)
        {
            Debug.LogError("Tile is null.");
            return null;
        }

        return await _board.CloneTile(tile, isActive);
    }

    public async UniTask ClearBoard()
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is null.");
            return;
        }

        var animTasks = new List<UniTask>();

        for (int x = 0; x < _board.BoardSize; x++)
        {
            for (int y = 0; y < _board.BoardSize; y++)
            {
                var cell = _board.Cells[x, y];
                if (cell != null && cell.Tile != null)
                {
                    if (cell.Tile is Tile tile)
                    {
                        animTasks.Add(AnimateAndReturnTile(tile));
                        cell.Tile = null;
                    }
                }
            }
        }

        await UniTask.WhenAll(animTasks);

        _positionChangedList.Clear();
        MatchedChains.Clear();
    }

    private async UniTask AnimateAndReturnTile(Tile tile)
    {
        await tile.transform.DOScale(1.2f, 0.12f)
            .SetEase(Ease.OutBack)
            .AsyncWaitForCompletion();

        await tile.transform.DOLocalMoveY(tile.transform.localPosition.y - 1f, 0.18f)
            .SetEase(Ease.InQuad)
            .AsyncWaitForCompletion();

        ObjectPooling.Instance.ReturnToPool(tile.gameObject);
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

