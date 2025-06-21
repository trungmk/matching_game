using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MatchSystem
{
    private static readonly Vector2Int[] _directions = new Vector2Int[]
    {
            new Vector2Int(0, 1),   
            new Vector2Int(1, 0),   
            new Vector2Int(0, -1),  
            new Vector2Int(-1, 0)   
    };

    public static List<Tile> CheckMatchAtPosition(Vector2Int boardPos, BoardController boardController)
    {
        if (!IsValidPosition(boardPos, boardController.Board.BoardSize))
        {
            Debug.LogError($"Invalid board position: {boardPos}.");
            return null;
        }

        Tile currentTile = boardController.GetTileByBoardPosition(boardPos) as Tile;

        if (currentTile == null)
        {
            return null;
        }

        int boardSize = boardController.Board.BoardSize;
        List<Tile> visitedTiles = new List<Tile>();
        visitedTiles.Add(currentTile);
        int index = 0;
        TileType checkType = currentTile.TileType;

        while (index < visitedTiles.Count)
        {
            currentTile = visitedTiles[index];
            for (int i = 0; i < _directions.Length; i++)
            {
                Vector2Int needToCheckPos = currentTile.BoardPosition + _directions[i];
                Tile potentialTile = GetPotentialTile(needToCheckPos, boardSize, checkType, boardController);

                if (potentialTile != null && !visitedTiles.Contains(potentialTile))
                {
                    visitedTiles.Add(potentialTile);
                }
            }

            index++;
        }

        return visitedTiles;
    }

    public static void CheckMatchInitializeBoard(BoardController boardController)
    {

    }

    private static Tile GetPotentialTile(Vector2Int boardPos, int boardSize, TileType tileType, BoardController boardController)
    {
        if (IsValidPosition(boardPos, boardSize))
        {
            Tile tile = boardController.GetTileByBoardPosition(boardPos) as Tile;

            if (tile != null && tileType == tile.TileType)
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
