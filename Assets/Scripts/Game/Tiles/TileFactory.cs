using Core;
using System.Threading.Tasks;
using UnityEngine;

public class TileFactory
{
    public async Task<Blocker> CreateBlocker(string blockerTypeString)
    {
        Blocker blocker = await ObjectPooling.Instance.Get<Blocker>("Blocker");

        if (blocker == null)
        {
            Debug.LogError("Could not get blocker from pool");
            return null;
        }

        BlockerType blockerType = ConvertBlockDataTypeToBlockerType(blockerTypeString);
        blocker.Setup(blockerType);

        return blocker;
    }

    public async Task<Tile> CreateTile(string tileTypeString)
    {
        Tile tile = await ObjectPooling.Instance.Get<Tile>("Tile");
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
