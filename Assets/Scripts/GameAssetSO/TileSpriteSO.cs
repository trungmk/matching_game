using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FruitTileSprite", menuName = "Game Asset/In Game/TileSprites")]
public class TileSpriteSO : ScriptableObject
{
    [SerializeField]
    private List<SpriteTileItem> _spriteTiles;

    [SerializeField]
    private List<SpriteBLockerItem> _blockerSpriteTiles;

    public Sprite GetSpriteByTileType(TileType type)
    {
        foreach (var item in _spriteTiles)
        {
            if (item.Type == type)
            {
                return item.Sprite;
            }
        }

        return null;
    }

    public Sprite GetSpriteByBlockerType(BlockerType type)
    {
        foreach (var item in _blockerSpriteTiles)
        {
            if (item.Type == type)
            {
                return item.Sprite;
            }
        }

        return null;
    }
}

[Serializable]
public class SpriteTileItem
{
    public TileType Type;

    public Sprite Sprite;
}

[Serializable]
public class SpriteBLockerItem
{
    public BlockerType Type;

    public Sprite Sprite;
}