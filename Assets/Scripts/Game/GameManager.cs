using NUnit.Framework;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

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
        if (isUseSocketData)
        {

            _gameStateMachine.Initialize();

            if (initCompleted != null)
            {
                initCompleted();
            }
        }
        else
        {
            BoardData boardData = await GeneratingTiles.GetGeneratedBoardData(_boardController);
            GameDataManager.Instance.UpdateBoardData(boardData, false);

            _gameStateMachine.Initialize();
            if (initCompleted != null)
            {
                initCompleted();
            }
        }

        //Timing.RunCoroutine(StartToRefreshBoardData());
    }

    public async void RefreshData()
    {
        await NetworkClient.Instance.SendSocketMessage("{\"action\":\"refresh_board\"}");
    }    

    private IEnumerator<float> StartToRefreshBoardData()
    {
        yield return Timing.WaitForSeconds(3f);

        RefreshData();
    }
}
