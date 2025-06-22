using NUnit.Framework;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;
using System.Threading.Tasks;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private BoardController _boardController;

    [SerializeField]
    private GameStateMachineMono _gameStateMachine;

    [SerializeField]
    private int boardSize = 10;

    [SerializeField]
    private bool isUseSocketData = false;

    public async UniTask InitGame(Action initCompleted)
    {
        if (GameDataManager.Instance.IsUseRemoteData)
        {
            if (GameDataManager.Instance.RemoteData == null)
            {
                GameDataManager.Instance.LocalData = await GeneratingTiles.GetGeneratedBoardData(_boardController);
                GameDataManager.Instance.RemoteData = GameDataManager.Instance.LocalData;
            }

            GameDataManager.Instance.UpdateBoardData(GameDataManager.Instance.RemoteData);
            _gameStateMachine.Initialize();
            initCompleted?.Invoke();
        }
        else
        {
            GameDataManager.Instance.LocalData = await GeneratingTiles.GetGeneratedBoardData(_boardController);
            GameDataManager.Instance.UpdateBoardData(GameDataManager.Instance.LocalData);

            _gameStateMachine.Initialize();
            initCompleted?.Invoke();
        }
    }
}
