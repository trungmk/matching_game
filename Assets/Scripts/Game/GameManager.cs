using NUnit.Framework;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public async UniTask InitGame(Action initCompleted)
    {
        if (isDebugUseSocketData)
        {
            _gameStateMachine.Initialize();

            if (initCompleted != null)
            {
                initCompleted();
            }
        }
        else
        {
            BoardData boardData = await GeneratingTiles.GenerateTile();
            GameDataManager.Instance.UpdateBoardData(boardData);

            if (initCompleted != null)
            {
                initCompleted();
            }
        }
    }
}
