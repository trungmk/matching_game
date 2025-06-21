using Core;
using DG.Tweening;
using System.Threading.Tasks;
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

    private void Awake()
    {
        _tileFactory = new TileFactory();
    }

    public void Initialize(BoardData boardData)
    {
        _boardSize = Mathf.Clamp(boardData.Size, _minSize, _maxSize);
        _cells = new BoardCell[_boardSize, _boardSize];
        
        CalculateSizes();
        CalculateBoardOriginPositionAtBottomLeft();
        InitCellBackground(_boardSize);
        InitBoard(boardData.Items);
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

    private async void InitCellBackground(int size)
    {
        for (int y = 0; y < _boardSize; y++)
        {
            for (int x = 0; x < _boardSize; x++)
            {
                CellBG cellBG = await ObjectPooling.Instance.Get<CellBG>("CellBG");
                cellBG.transform.SetParent(_cellBGHolder);
                cellBG.transform.localPosition = new Vector2(x * _cellBGSize, y * _cellBGSize);
                cellBG.transform.localScale = new Vector2(_cellBGSize, _cellBGSize);
                cellBG.SetColor((x + y) % 2 == 0);
                cellBG.gameObject.SetActive(true);
            }
        }
    }

    private async void InitBoard(BoardItem[][] boardItems)
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

    public BaseTile GetTileFromWorldPosition(Vector2 worldPosition)
    {
        Vector2Int tilePosition = GetTilePositionFromWorldPosition(worldPosition);

        if (tilePosition == _outOfBound)
        {
            return null; 
        }

        return _cells[tilePosition.x, tilePosition.y].Tile;
    }

    public void SwapTiles(BaseTile firstTile, BaseTile secondTile)
    {
        if (firstTile == null || secondTile == null)
        {
            Debug.LogError("One or both tiles are null.");
            return;
        }

        Vector2 nextFirstPos = secondTile.transform.localPosition;
        Vector2 nextSecondPos = firstTile.transform.localPosition;

        firstTile.transform.DOLocalMove(nextFirstPos, 0.12f);
        secondTile.transform.DOLocalMove(nextSecondPos, 0.12f);

        Vector2Int firstPos = firstTile.BoardPosition;
        Vector2Int secondPos = secondTile.BoardPosition;

        firstTile.BoardPosition = secondPos;
        secondTile.BoardPosition = firstPos;

        _cells[firstPos.x, firstPos.y].Tile = secondTile;
        _cells[secondPos.x, secondPos.y].Tile = firstTile;
    }

    public BaseTile GetTileByBoardPosition(Vector2Int boardPosition)
    {
        if (boardPosition.x < 0 || boardPosition.x >= _boardSize || 
            boardPosition.y < 0 || boardPosition.y >= _boardSize)
        {
            return null;
        }

        return _cells[boardPosition.x, boardPosition.y].Tile;
    }
}