using Core;
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

    private int _boardSize = 10;

    private BoardCell[,] _cells;

    private float _cellSize;

    private float _cellBGSize;

    private Vector2 bottomLeftWorldPosition;

    public void Initialize(BoardData boardData)
    {
        _boardSize = Mathf.Clamp(boardData.Size, _minSize, _maxSize);
        _cells = new BoardCell[_boardSize, _boardSize];
        
        CalculateSizes();
        CalculateBoardOriginPositionAtBottomLeft();
        InitCellBackground(_boardSize);
        //InitBoard(boardData.Items);
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
        bottomLeftWorldPosition.y -= 1.5f;
        _cellBGHolder.position = bottomLeftWorldPosition;
    }

    private async void InitCellBackground(int size)
    {
        for (int y = 0; y < _boardSize; y++)
        {
            for (int x = 0; x < _boardSize; x++)
            {
                CellBG cellBG = await ObjectPooling.Instance.Get<CellBG>("CellBG");
                cellBG.transform.SetParent(_cellBGHolder);
                cellBG.transform.localPosition = new Vector3(x * _cellBGSize, y * _cellBGSize, 0f);
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
                
                Tile tile = null;
                BoardItem boardItemAtCell = boardItemRow[x];
                if (boardItemAtCell.Type.Equals("X"))
                {
                    tile = await CreateBlocker(boardItemAtCell.Type);
                    _cells[x, y].IsBlocker = true;
                }
                else
                {
                    tile = await CreateTile(boardItemAtCell.Type);
                }

                _cells[x, y].IsEnable = true;
                _cells[x, y].Tile = tile;
                _cells[x, y].Position = new Vector2Int(x, y);
            }
        }
    }

    private async Task<Blocker> CreateBlocker(string blockerTypeString)
    {
        Blocker blocker = await ObjectPooling.Instance.Get<Blocker>("Blocker");
        BlockerType blockerType = ConvertBlockDataTypeToBlockerType(blockerTypeString);
        blocker.Setup(blockerType);

        return blocker;
    }

    private async Task<Tile> CreateTile(string tileTypeString)
    {
        Tile tile = await ObjectPooling.Instance.Get<Tile>("Tile");
        TileType tileType = ConvertBlockDataTypeToTileType(tileTypeString);
        tile.Setup(tileType);

        return null;
    }

    private TileType ConvertBlockDataTypeToTileType(string tileType)
    {
        switch (tileType)
        {
            case "A":
                return TileType.A;
            case "B":
                return TileType.B;
            case "C":
                return TileType.C;
            case "D":
                return TileType.D;
            case "E":
                return TileType.E;
            default:
                // Default choose A
                return TileType.A;
        }
    }

    private BlockerType ConvertBlockDataTypeToBlockerType(string blockerType)
    {
        switch (blockerType)
        {
            case "X":
                return BlockerType.X;
            default:
                // Default choose A
                return BlockerType.X;
        }
    }
}