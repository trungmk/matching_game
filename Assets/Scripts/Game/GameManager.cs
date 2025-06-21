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
    private GameStateMachineMono _gameStateMachine;

    [SerializeField]
    private int boardSize = 10;

    [SerializeField]
    private bool isDebugUseSocketData = false;

    public void InitGame(Action initCompleted)
    {
        if (isDebugUseSocketData)
        {
            _gameStateMachine.Initialize(false);
        }
        else
        {
            GeneratingTiles.GenerateTile();
            //_boardController.InitializeBoard(new BoardData
            //{
            //    Size = boardData.Size,
            //});

            _gameStateMachine.Initialize(true);
        }

        

        if (initCompleted != null)
        {
            initCompleted();
        }
    }
}
