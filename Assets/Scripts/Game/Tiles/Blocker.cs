using UnityEngine;

public class Blocker : BaseTile
{
	protected BlockerType _blockerType;

	public BlockerType BlockerType => _blockerType;

    public void Setup(BlockerType blockerType)
    {
        Sprite tileSprite = _tileSpriteSO.GetSpriteByBlockerType(blockerType);

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