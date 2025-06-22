using Core;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BootingSceneController : SceneController
{
    public override void OnLoaded()
    {
        ObjectPooling.Instance.OnLoadPoolsCompleted += Handle_OnloadPoolCompleted;
        GameDataManager.Instance.OnBoardDataUpdated += HandleBoardDataUpdated;

        ObjectPooling.Instance.Init(null);

        UIManager.Instance.Show<SplashScreenTransition>();

        // Configure the WebSocket and connect
        string sessionId = System.Guid.NewGuid().ToString();
        NetworkClient.Instance.SetSessionId(sessionId);
        NetworkClient.Instance.ConnectWebSocket();
    }

    public override void OnUnloaded()
    {
        ObjectPooling.Instance.OnLoadPoolsCompleted -= Handle_OnloadPoolCompleted;
        GameDataManager.Instance.OnBoardDataUpdated -= HandleBoardDataUpdated;
    }

    private void HandleBoardDataUpdated(BoardData data)
    {
        
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForOneFrame;
        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_MENU);
    }

    private void Handle_OnloadPoolCompleted()
    {
        Timing.RunCoroutine(ChangeScene());
    }
}
