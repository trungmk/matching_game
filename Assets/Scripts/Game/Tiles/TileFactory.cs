using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TileFactory
{
    private const string TILE_ADDRESS = "Tile";
    private const string BLOCKER_ADDRESS = "Blocker";

    public async UniTask<Blocker> CreateBlocker(string blockerTypeString)
    {
        Blocker blocker = await ObjectPooling.Instance.Get<Blocker>(BLOCKER_ADDRESS);

        if (blocker == null)
        {
            Debug.LogError("Could not get blocker from pool");
            return null;
        }

        BlockerType blockerType = ConvertBlockDataTypeToBlockerType(blockerTypeString);
        blocker.Setup(blockerType);

        return blocker;
    }

    public async UniTask<Tile> CreateTile(string tileTypeString)
    {
        Tile tile = await ObjectPooling.Instance.Get<Tile>(TILE_ADDRESS);
        if (tile == null)
        {
            Debug.LogError("Could not get tile from pool");
            return null;
        }

        TileType tileType = ConvertBlockDataTypeToTileType(tileTypeString);
        tile.Setup(tileType);

        return tile;
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
