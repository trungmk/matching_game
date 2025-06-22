using Core;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardMono : MonoBehaviour
{
    [SerializeField]
    private int _minSize = 5;

    [SerializeField]
    private int _maxSize = 13;

    [SerializeField]
    private float _boardDisplaySize = 11f;

    [SerializeField]
    private float _minCellSize = 0.8f;

    [SerializeField]
    private float _maxCellSize = 2f;

    [SerializeField]
    private float _minCellBG = 0.8f;

    [SerializeField]
    private float _maxCellBG = 2f;

    [SerializeField]
    private Transform _cellBGHolder;

    [SerializeField]
    private Transform _cellsHolder;

    [SerializeField]
    private float _yOffset = 1.5f;

    private int _boardSize = 10;

    private BoardCell[,] _cells;

    private float _cellSize;

    private float _cellBGSize;

    private Vector2 bottomLeftWorldPosition;

    private TileFactory _tileFactory;

    private Vector2Int _outOfBound = new Vector2Int(-1, -1);

    public int BoardSize => _boardSize;

    public BoardCell[,] Cells => _cells;

    private const string CELL_BG_ADDRESS = "CellBG";

    private void Awake()
    {
        _tileFactory = new TileFactory();
    }

    public async UniTask Initialize(BoardData boardData)
    {
        _boardSize = Mathf.Clamp(boardData.Size, _minSize, _maxSize);
        _cells = new BoardCell[_boardSize, _boardSize];

        CalculateSizes();
        CalculateBoardOriginPositionAtBottomLeft();
        await InitCellBackground(_boardSize);
        await InitBoard(boardData.Items);
    }

    private void CalculateSizes()
    {
        if (_boardSize <= 0)
        {
            Debug.LogError("Board size must be greater than zero.");
            return;
        }

        if (_boardSize <= _boardDisplaySize)
        {
            float additionalComplement = _boardDisplaySize - _boardSize;
            additionalComplement /= _boardDisplaySize;
            _cellSize = Mathf.Clamp(1f + additionalComplement, _minCellSize, _maxCellSize); 
        }
        else
        {
            float additionalComplement = _boardSize - _boardDisplaySize;
            additionalComplement /= _boardDisplaySize;
            _cellSize = Mathf.Clamp(1f - additionalComplement, _minCellSize, _maxCellSize);
        }

        _cellBGSize = _cellSize;
    }

    private void CalculateBoardOriginPositionAtBottomLeft()
    {
        float totalSize = _cellSize * (_boardSize - 1);
        bottomLeftWorldPosition = new Vector2(-totalSize / 2f, -totalSize / 2f);
        bottomLeftWorldPosition.y -= _yOffset;
        _cellBGHolder.localPosition = bottomLeftWorldPosition;
        _cellsHolder.localPosition = bottomLeftWorldPosition;
    }

    private async UniTask InitCellBackground(int size)
    {
        for (int y = 0; y < _boardSize; y++)
        {
            for (int x = 0; x < _boardSize; x++)
            {
                CellBG cellBG = await ObjectPooling.Instance.Get<CellBG>(CELL_BG_ADDRESS);
                cellBG.transform.SetParent(_cellBGHolder);
                cellBG.transform.localPosition = new Vector2(x * _cellBGSize, y * _cellBGSize);
                cellBG.transform.localScale = new Vector2(_cellBGSize, _cellBGSize);
                cellBG.SetColor((x + y) % 2 == 0);
                cellBG.gameObject.SetActive(true);
            }
        }
    }

    public async UniTask InitBoard(BoardItem[][] boardItems)
    {
        for (int y = 0; y < boardItems.Length; y++)
        {
            BoardItem[] boardItemRow = boardItems[y];
            for (int x = 0; x < boardItemRow.Length; x++)
            {
                _cells[x, y] = new BoardCell();
                
                BaseTile tile = null;
                BoardItem boardItemAtCell = boardItemRow[x];
                if (boardItemAtCell.Type.Equals("X"))
                {
                    tile = await _tileFactory.CreateBlocker(boardItemAtCell.Type);
                    _cells[x, y].IsBlocker = true;
                }
                else
                {
                    tile = await _tileFactory.CreateTile(boardItemAtCell.Type);
                }

                _cells[x, y].IsEnable = true;
                _cells[x, y].Tile = tile;
                _cells[x, y].LocalPosition = new Vector2(x * _cellSize, y * _cellSize);
                _cells[x, y].BoardPosition = new Vector2Int(x, y);

                tile.transform.SetParent(_cellsHolder);
                tile.BoardPosition = new Vector2Int(x, y);
                tile.transform.localPosition = _cells[x, y].LocalPosition;
                tile.transform.localScale = new Vector2(_cellSize, _cellSize);
            }
        }
    }

    public Vector2Int GetTilePositionFromWorldPosition(Vector2 worldPosition)
    {
        Vector3 localPos = _cellsHolder.InverseTransformPoint(worldPosition);
        int x = Mathf.RoundToInt(localPos.x / _cellSize);
        int y = Mathf.RoundToInt(localPos.y / _cellSize);

        if (x < 0 || x >= _boardSize || y < 0 || y >= _boardSize)
        {
            return _outOfBound; 
        }

        return new Vector2Int(x, y);
    }

    public T GetTileFromWorldPosition<T>(Vector2 worldPosition) where T : BaseTile
    {
        Vector2Int tilePosition = GetTilePositionFromWorldPosition(worldPosition);

        if (tilePosition == _outOfBound)
        {
            return null; 
        }

        if (_cells[tilePosition.x, tilePosition.y].Tile == null)
        {
            return null;
        }

        return _cells[tilePosition.x, tilePosition.y].Tile as T;
    }

    public Vector3 GetWorldPositionFromLocalPosition(Vector2 localPosition)
    {
        Vector3 worldPos = _cellsHolder.TransformPoint(localPosition);
        //worldPos.y += _yOffset; 
        return worldPos;
    }

    public void SwapTiles(BaseTile firstTile, BaseTile secondTile)
    {
        if (firstTile == null || secondTile == null)
        {
            Debug.LogError("One or both tiles are null.");
        }

        if (firstTile.IsLocked || secondTile.IsLocked)
        {
            Debug.LogError("One or both tiles are locked.");
        }

        Vector2Int firstPos = firstTile.BoardPosition;
        Vector2Int secondPos = secondTile.BoardPosition;

        firstTile.BoardPosition = secondPos;
        secondTile.BoardPosition = firstPos;

        _cells[firstPos.x, firstPos.y].Tile = secondTile;
        _cells[secondPos.x, secondPos.y].Tile = firstTile;

        Vector2 nextFirstPos = secondTile.transform.localPosition;
        Vector2 nextSecondPos = firstTile.transform.localPosition;

        firstTile.transform.DOLocalMove(nextFirstPos, 0.1f);
        secondTile.transform.DOLocalMove(nextSecondPos, 0.1f);
    }

    public T GetTileByBoardPosition<T>(Vector2Int boardPosition) where T : BaseTile
    {
        if (boardPosition.x < 0 || boardPosition.x >= _boardSize || 
            boardPosition.y < 0 || boardPosition.y >= _boardSize)
        {
            return null;
        }

        if (_cells[boardPosition.x, boardPosition.y].Tile == null)
        {
            return null;
        }

        return _cells[boardPosition.x, boardPosition.y].Tile as T;
    }

    public BoardCell GetCellByBoardPosition(Vector2Int boardPosition)
    {
        if (boardPosition.x < 0 || boardPosition.x >= _boardSize || 
            boardPosition.y < 0 || boardPosition.y >= _boardSize)
        {
            return null;
        }

        return _cells[boardPosition.x, boardPosition.y];
    }

    public async UniTask<Tile> CreateRandomTile()
    {
        TileType randomType = (TileType) UnityEngine.Random.Range(0, (int) TileType.Length);
        Tile tile = await _tileFactory.CreateTile(randomType.ToString());
        tile.transform.SetParent(_cellsHolder);
        return tile;
    }

    public async UniTask<Tile> CloneTile(Tile tileToClone, bool isActive)
    {
        if (tileToClone == null)
        {
            Debug.LogError("Tile to clone is null.");
            return null;
        }

        Tile clonedTile = await _tileFactory.CreateTile(tileToClone.TileType.ToString());
        clonedTile.gameObject.SetActive(false);
        clonedTile.transform.SetParent(_cellsHolder);
        clonedTile.BoardPosition = tileToClone.BoardPosition;
        clonedTile.transform.localPosition = tileToClone.transform.localPosition;
        clonedTile.transform.localScale = tileToClone.transform.localScale;
        return clonedTile;
    }
}