using System.Collections.Generic;
using System.Threading.Tasks;
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

    public static HashSet<Tile> CheckMatchAtPosition(Tile tile, BoardController boardController)
    {
        if (tile == null)
        {
            return null;
        }

        Tile currentTile = tile;

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
                Tile potentialTile = GetPotentialMatchingTile(needToCheckPos, boardSize, checkType, boardController);

                if (potentialTile != null && !matchingGroup.Contains(potentialTile))
                {
                    matchingGroup.Add(potentialTile);
                    tilesToCheck.Enqueue(potentialTile);
                }
            }
        }

        return matchingGroup;
    }

    public static HashSet<Tile> FindPotentialMatchesForTile(BoardController boardController)
    {
        int boardSize = boardController.Board.BoardSize;
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                Vector2Int posA = new Vector2Int(x, y);
                Tile tileA = boardController.GetTileByBoardPosition<Tile>(posA);
                if (tileA == null || tileA is Blocker)
                {
                    continue;
                }

                for (int i = 0; i < _directions.Length; i++)
                {
                    Vector2Int posB = posA + _directions[i];
                    if (!IsValidPosition(posB, boardSize))
                    {
                        continue;
                    }

                    Tile tileB = boardController.GetTileByBoardPosition<Tile>(posB);
                    if (tileB == null || tileB is Blocker)
                    {
                        continue;
                    }

                    // Swap tiles in board
                    boardController.Board.Cells[x, y].Tile = tileB;
                    boardController.Board.Cells[posB.x, posB.y].Tile = tileA;

                    // Save old positions
                    Vector2Int oldPosA = tileA.BoardPosition;
                    Vector2Int oldPosB = tileB.BoardPosition;
                    tileA.BoardPosition = posB;
                    tileB.BoardPosition = posA;

                    // Check for match at both positions
                    var matchA = CheckMatchAtPosition(tileA, boardController);
                    var matchB = CheckMatchAtPosition(tileB, boardController);

                    // Swap back
                    boardController.Board.Cells[x, y].Tile = tileA;
                    boardController.Board.Cells[posB.x, posB.y].Tile = tileB;
                    tileA.BoardPosition = oldPosA;
                    tileB.BoardPosition = oldPosB;

                    if (matchA != null && matchA.Count >= 3)
                    {
                        return matchA;
                    }    
                        
                    if (matchB != null && matchB.Count >= 3)
                    {
                        return matchB;
                    }    
                }
            }
        }
        return null;
    }

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

                HashSet<Tile> tiles = CheckMatchAtPosition(tile, boardController);
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

    private static Tile GetPotentialMatchingTile(Vector2Int boardPos, int boardSize, TileType tileType, BoardController boardController)
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
