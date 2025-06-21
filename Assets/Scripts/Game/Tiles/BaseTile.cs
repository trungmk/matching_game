using Core;
using UnityEngine;

public class BaseTile : PooledMono
{
    [SerializeField]
    protected SpriteRenderer _tileSpriteRenderer;

    [SerializeField]
    protected TileSpriteSO _tileSpriteSO;

    private bool _isLocked;

    private Vector2Int _boardPosition;

    public bool IsLocked { get { return _isLocked; } }

    public Vector2Int BoardPosition { get { return _boardPosition; } set { _boardPosition = value; } }
}
