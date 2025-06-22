using System.Collections.Generic;
using UnityEngine;

public class MatchSystem
{
    private static readonly Vector2Int[] _directions = new Vector2Int[]
    {
            new Vector2Int(0, 1),   
            new Vector2Int(1, 0),   
            new Vector2Int(0, -1),  
            new Vector2Int(-1, 0)   
    };

    public static HashSet<Tile> CheckMatchAtPosition(Vector2Int boardPos, BoardController boardController)
    {
        if (!IsValidPosition(boardPos, boardController.Board.BoardSize))
        {
            Debug.LogError($"Invalid board position: {boardPos}.");
            return null;
        }

        Tile currentTile = boardController.GetTileByBoardPosition<Tile>(boardPos);

        if (currentTile == null)
        {
            return null;
        }

        int boardSize = boardController.Board.BoardSize;
        HashSet<Tile> matchingGroup = new HashSet<Tile>();
        Queue<Tile> tilesToCheck = new Queue<Tile>();

        matchingGroup.Add(currentTile);
        tilesToCheck.Enqueue(currentTile);
        TileType checkType = currentTile.TileType;

        while (tilesToCheck.Count > 0)
        {
            currentTile = tilesToCheck.Dequeue();
            
            for (int i = 0; i < _directions.Length; i++)
            {
                Vector2Int needToCheckPos = currentTile.BoardPosition + _directions[i];
                Tile potentialTile = GetPotentialTile(needToCheckPos, boardSize, checkType, boardController);

                if (potentialTile != null && !matchingGroup.Contains(potentialTile))
                {
                    matchingGroup.Add(potentialTile);
                    tilesToCheck.Enqueue(potentialTile);
                }
            }
        }

        return matchingGroup;
    }

    //public static Dictionary<MatchPattern, HashSet<Tile>> GetAllPotentialMatching()
    //{



    //    return new Dictionary<MatchPattern, HashSet<Tile>>
    //    {
    //        { MatchPattern.Horizontal, new HashSet<Tile>() },
    //        { MatchPattern.Vertical, new HashSet<Tile>() },
    //        { MatchPattern.LShape, new HashSet<Tile>() },
    //        { MatchPattern.TShape, new HashSet<Tile>() },
    //        { MatchPattern.CrossShape, new HashSet<Tile>() }
    //    };
    //}

    public static List<HashSet<Tile>> CheckMatchWholeBoard(BoardController boardController)
    {
        List<HashSet<Tile>> matchedChains = new List<HashSet<Tile>>();
        BoardMono board = boardController.Board;
        int boardSize = board.BoardSize;
        Vector2Int currentPosition = new Vector2Int(0, 0);

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                currentPosition.x = x;
                currentPosition.y = y;
                Tile tile = boardController.GetTileByBoardPosition<Tile>(currentPosition);

                if (tile == null)
                    continue;

                if (IsTileInMatchedChains(matchedChains, tile))
                {
                    continue;
                }

                HashSet<Tile> tiles = CheckMatchAtPosition(currentPosition, boardController);
                if (tiles.Count >= 3)
                {
                    matchedChains.Add(tiles);
                }
            }
        }

        return matchedChains;
    }

    private static bool IsTileInMatchedChains(List<HashSet<Tile>> tiles, Tile tile)
    {
        foreach (var chain in tiles)
        {
            if (chain.Contains(tile))
            {
                return true;
            }
        }

        return false;
    }

    private static Tile GetPotentialTile(Vector2Int boardPos, int boardSize, TileType tileType, BoardController boardController)
    {
        if (IsValidPosition(boardPos, boardSize))
        {
            Tile tile = boardController.GetTileByBoardPosition<Tile>(boardPos);

            if (tile != null && tile.TileType == tileType)
            {
                return tile;
            }
        }

        return null;
    }

    private static bool IsValidPosition(Vector2Int pos, float boardSize)
    {
        return pos.x >= 0 && pos.x < boardSize && pos.y >= 0 && pos.y < boardSize;
    }
}
