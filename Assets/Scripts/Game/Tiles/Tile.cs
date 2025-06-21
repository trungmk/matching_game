using Core;
using UnityEngine;

public class Tile : BaseTile
{
    public TileType TileType { get; private set; }

    public void Setup(TileType tileType)
    {
        TileType = TileType;
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
