using Core;
using UnityEngine;

public class Tile : PooledMono
{
    [SerializeField]
    protected SpriteRenderer _tileSpriteRenderer;

    [SerializeField]
    protected TileSpriteSO _tileSpriteSO;

    private bool _isLocked;

    private Vector2Int _position;

    public bool IsLocked { get { return _isLocked; } }

    public Vector2Int Position { get { return _position; } set { _position = value; } }

    public virtual void Setup(TileType tileType)
    {
        Sprite tileSprite = _tileSpriteSO.GetSpriteByTileType(tileType);

        if (_tileSpriteRenderer != null && tileSprite != null)
        {
            _tileSpriteRenderer.sprite = tileSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer component is missing on the Tile GameObject.");
        }
    }
}
