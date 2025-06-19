using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private BoardController _boardController;

    [SerializeField]
    private int boardSize = 10;

    public async Task InitGame(Action initCompleted)
    {
        BoardData boardData = GameDataManager.Instance.CurrentBoardData;

        Debug.Log(boardData);
        GeneratingTiles.GenerateTile();
        _boardController.InitializeBoard(new BoardData
        {
            Size = boardData.Size,
        });

        if (initCompleted != null)
        {
            initCompleted();
        }
    }
}
