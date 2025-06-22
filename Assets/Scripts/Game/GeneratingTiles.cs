using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GeneratingTiles
{
    private static string _TileTemplateAddress = "BoardTemplate10x10";

    private static readonly Vector2Int[] _directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    public static async UniTask<BoardData> GetGeneratedBoardData(BoardController boardController)
    {
        BoardData boardData = null;
        List<List<BoardItem>> matchedItems = null;
        HashSet<BoardItem> potentialMatches = null;
        int maxTries = 10;
        int tryCount = 0;
        do
        {
            boardData = await GenerateBoardItems(boardController);
            matchedItems = CheckMatchWholeBoard(boardData);
            potentialMatches = FindPotentialMatchesForTile(boardData);
            tryCount++;
        } while ((matchedItems.Count > 0 || potentialMatches == null) && tryCount < maxTries);
    
        return boardData;
    }

    public static async UniTask<BoardData> GenerateBoardItems(BoardController boardController)
    {
        BoardMessage boardMessage = await AssetManager.Instance.LoadFromTextAssetAsync<BoardMessage>(_TileTemplateAddress);

        if (boardMessage == null)
        {
            Debug.LogError("Could not load BoardMessage");
            return null;
        }

        int boardSize = boardMessage.Board.Size;
        BoardData boardDataWithRandomTiles = FillBoardWithRandomTiles(boardMessage.Board);
        List<List<BoardItem>> matchedItems = CheckMatchWholeBoard(boardDataWithRandomTiles);

        if (matchedItems.Count > 0)
        {
            for (int i = 0; i < matchedItems.Count; i++)
            {
                List<BoardItem> matchedBoardItems = matchedItems[i];

                // ex: if match length is 5, so 5-2 = 3 items need to swap.
                int numberItemNeedToSwap = matchedBoardItems.Count - 2;

                foreach (var item in matchedBoardItems)
                {
                    bool wasSwapped = false;
                    for (int j = 0; j < _directions.Length; j++)
                    {
                        Vector2Int swapPos = new Vector2Int(item.X + _directions[j].x, item.Y + _directions[j].y);

                        if (!IsValidPosition(swapPos, boardSize))
                        {
                            continue;
                        }

                        BoardItem swapedBoardItem = boardDataWithRandomTiles.Items[swapPos.y][swapPos.x];
                        if (item.Type == swapedBoardItem.Type)
                        {
                            continue;
                        }

                        string swapTileType = swapedBoardItem.Type;
                        swapedBoardItem.Type = item.Type;
                        item.Type = swapTileType;
                        wasSwapped = true;
                        break;
                    }

                    if (!wasSwapped)
                    {
                        TileType tileType;
                        if (Enum.TryParse<TileType>(item.Type, out tileType))
                        {
                            int min = 0;
                            int max = (int) TileType.Length;
                            int excluded = (int) tileType;

                            int random;
                            do
                            {
                                random = UnityEngine.Random.Range(min, max); 
                            } while (random == excluded);

                            TileType randomTile = (TileType)random;
                            item.Type = randomTile.ToString();
                        }
                    }

                    if (--numberItemNeedToSwap < 0)
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            List<Vector2Int> potentialMatchedChainPosition = GetPotentialMatchChainPositions(boardDataWithRandomTiles);
            Vector2Int pos = potentialMatchedChainPosition[0];
            BoardItem boardItem = boardDataWithRandomTiles.Items[pos.y][pos.x];
            string tileType = boardItem.Type;

            for (int i = 1; i < potentialMatchedChainPosition.Count; i++)
            {
                pos = potentialMatchedChainPosition[i];
                boardItem.Type = tileType;
            }

        }

        return boardDataWithRandomTiles;
    }

    private static List<Vector2Int> GetPotentialMatchChainPositions(BoardData boardData)
    {
        bool isDone = false;
        Vector2Int centerPos = Vector2Int.zero;
        Vector2Int leftPos = Vector2Int.zero;
        Vector2Int rightDownPos = Vector2Int.zero; 

        while (isDone)
        {
            int x = UnityEngine.Random.Range(0, boardData.Size);
            int y = UnityEngine.Random.Range(0, boardData.Size);
            centerPos = new Vector2Int(x, y);

            BoardItem boardItem = boardData.Items[centerPos.y][centerPos.x];
            if (boardItem.Type.Equals("X"))
            {
                continue;
            }

            string tileType = boardItem.Type;
            leftPos = new Vector2Int(centerPos.x - 1, centerPos.y);
            rightDownPos = new Vector2Int(centerPos.x + 1, centerPos.y - 1);

            if (IsValidPosition(leftPos, boardData.Size))
            {
                BoardItem leftItem = boardData.Items[leftPos.y][leftPos.x];
                if (leftItem.Type.Equals("X"))
                {
                    continue;
                }
            }

            if (IsValidPosition(rightDownPos, boardData.Size))
            {
                BoardItem RightDownItem = boardData.Items[rightDownPos.y][rightDownPos.x];
                if (RightDownItem.Type.Equals("X"))
                {
                    continue;
                }
            }

            break;
        }

        return new List<Vector2Int> { centerPos, leftPos, rightDownPos };
    }


    private static BoardData FillBoardWithRandomTiles(BoardData boardData)
    {
        int boardSize = boardData.Size;
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (boardData.Items[y][x].Type.Equals("X"))
                {
                    continue;
                }

                TileType tileType = (TileType)UnityEngine.Random.Range(0, (int)TileType.Length);
                boardData.Items[y][x].Type = tileType.ToString();
            }
        }

        return boardData;
    }

    private static HashSet<BoardItem> CheckMatchAtPosition(BoardItem boardItem, BoardData boardData)
    {
        if (boardItem == null || boardItem.Type == "X" || string.IsNullOrEmpty(boardItem.Type))
        {
            return null;
        }

        int boardSize = boardData.Size;
        HashSet<BoardItem> matchingGroup = new HashSet<BoardItem>();
        Queue<BoardItem> itemsToCheck = new Queue<BoardItem>();

        matchingGroup.Add(boardItem);
        itemsToCheck.Enqueue(boardItem);
        string checkType = boardItem.Type;

        while (itemsToCheck.Count > 0)
        {
            var current = itemsToCheck.Dequeue();
            int cx = current.X;
            int cy = current.Y;

            for (int i = 0; i < _directions.Length; i++)
            {
                int nx = cx + _directions[i].x;
                int ny = cy + _directions[i].y;
                if (nx < 0 || nx >= boardSize || ny < 0 || ny >= boardSize)
                    continue;
                var neighbor = boardData.Items[ny][nx];
                if (neighbor != null && neighbor.Type == checkType && neighbor.Type != "X" && !matchingGroup.Contains(neighbor))
                {
                    matchingGroup.Add(neighbor);
                    itemsToCheck.Enqueue(neighbor);
                }
            }
        }

        return matchingGroup;
    }

    public static List<List<BoardItem>> CheckMatchWholeBoard(BoardData boardData)
    {
        var matchedChains = new List<List<BoardItem>>();
        int boardSize = boardData.Size;
        var visited = new HashSet<BoardItem>();

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                var item = boardData.Items[y][x];
                if (item == null || item.Type == "X" || string.IsNullOrEmpty(item.Type) || visited.Contains(item))
                {
                    continue;
                }    
                    
                var match = CheckMatchAtPosition(item, boardData);
                if (match.Count >= 3)
                {
                    matchedChains.Add(new List<BoardItem>(match));
                    foreach (var m in match)
                    {
                        visited.Add(m);
                    }
                }
            }
        }

        return matchedChains;
    }

    private static HashSet<BoardItem> FindPotentialMatchesForTile(BoardData boardData)
    {
        int boardSize = boardData.Size;
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                BoardItem itemA = boardData.Items[y][x];
                if (itemA == null || itemA.Type == "X")
                    continue;

                for (int i = 0; i < _directions.Length; i++)
                {
                    int nx = x + _directions[i].x;
                    int ny = y + _directions[i].y;
                    if (!IsValidPosition(new Vector2Int(nx, ny), boardSize))
                        continue;

                    BoardItem itemB = boardData.Items[ny][nx];
                    if (itemB == null || itemB.Type == "X")
                    {
                        continue;
                    }

                    // Swap type
                    string tempType = itemA.Type;
                    itemA.Type = itemB.Type;
                    itemB.Type = tempType;

                    HashSet<BoardItem> matchA = CheckMatchAtPosition(itemA, boardData);
                    if (matchA != null && matchA.Count >= 3)
                    {
                        return matchA;
                    }

                    HashSet<BoardItem> matchB = CheckMatchAtPosition(itemB, boardData);
                    if (matchB != null && matchB.Count >= 3)
                    {
                        return matchB;
                    }

                    // Swap back
                    tempType = itemA.Type;
                    itemA.Type = itemB.Type;
                    itemB.Type = tempType;
                }
            }
        }

        return null;
    }

    private static bool IsValidPosition(Vector2Int pos, float boardSize)
    {
        return pos.x >= 0 && pos.x < boardSize && pos.y >= 0 && pos.y < boardSize;
    }
}