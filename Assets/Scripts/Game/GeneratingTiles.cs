using Core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GeneratingTiles
{
    private static string _TileTemplateAddress = "BoardTemplate10x10";

    public static async UniTask<BoardData> GetGeneratedBoardData(BoardController boardController)
    {
        return await GenerateBoardItems(boardController);
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
        return boardDataWithRandomTiles;
    }

    private static BoardData FillBoardWithRandomTiles(BoardData boardData)
    {
        int boardSize = boardData.Size;
        int maxTries = 10; 

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (boardData.Items[y][x].Type.Equals("X"))
                {
                    continue;
                }

               
                List<string> neighbordjacentTypes = GetNeighborTypes(boardData, x, y);
                //int count = 0;
                //while (count < maxTries)
                //{
                //    count++;
                //    TileType tileType = GetRandomNonMatchingTileType(adjacentTypes);
                //    boardData.Items[y][x].Type = tileType.ToString();
                //    HashSet<BoardItem> boardItems = CheckMatchAtPosition(boardData.Items[y][x], boardData);

                //    if (boardItems == null || boardItems.Count == 0)
                //    {
                //        break;
                //    }
                //}

                TileType tileType = GetRandomNonMatchingTileType(neighbordjacentTypes);
                boardData.Items[y][x].Type = tileType.ToString();
            }
        }

        return boardData;
    }

    private static List<string> GetNeighborTypes(BoardData boardData, int x, int y)
    {
        List<string> neighborTypes = new List<string>();
        int boardSize = boardData.Size;

        // Add tile type of the left tile.
        if (x >= 1)
        {
            if (!boardData.Items[y][x - 1].Type.Equals("X"))
            {
                neighborTypes.Add(boardData.Items[y][x - 1].Type);
            }
        }

        // Add tile type of the below tile.
        if (y >= 1)
        {
            if (!boardData.Items[y - 1][x].Type.Equals("X"))
            {
                neighborTypes.Add(boardData.Items[y - 1][x].Type);
            }
        }

        return neighborTypes;
    }

    private static TileType GetRandomNonMatchingTileType(List<string> adjacentTypes)
    {
        int tileTypeCount = (int) TileType.Length;
        List<TileType> availableTypes = new List<TileType>();

        // exclude neighbor tile types
        for (int i = 0; i < tileTypeCount; i++)
        {
            TileType type = (TileType) i;
            if (!adjacentTypes.Contains(type.ToString()))
            {
                availableTypes.Add(type);
            }
        }

        if (availableTypes.Count == 0)
        {
            return (TileType) UnityEngine.Random.Range(0, tileTypeCount);
        }

        return availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
    }
}