using Core;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BootingSceneController : SceneController
{
    private void OnEnable()
    {
        ObjectPooling.Instance.OnLoadPoolsCompleted += Handle_OnloadPoolCompleted;
        GameDataManager.Instance.OnBoardDataUpdated += HandleBoardDataUpdated;
    }

    private void OnDisable()
    {
        ObjectPooling.Instance.OnLoadPoolsCompleted -= Handle_OnloadPoolCompleted;
        GameDataManager.Instance.OnBoardDataUpdated -= HandleBoardDataUpdated;
    }

    public override void OnLoaded()
    {
        ObjectPooling.Instance.Init(null);

        UIManager.Instance.Show<SplashScreenTransition>();

        // Configure the WebSocket and connect
        string sessionId = System.Guid.NewGuid().ToString();
        NetworkClient.Instance.SetSessionId(sessionId);
        NetworkClient.Instance.ConnectWebSocket();
    }

    private void HandleBoardDataUpdated(BoardData data)
    {
        
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForSeconds(2f); 
        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_GAME);
    }

    private void Handle_OnloadPoolCompleted()
    {
        Timing.RunCoroutine(ChangeScene());
    }
}
