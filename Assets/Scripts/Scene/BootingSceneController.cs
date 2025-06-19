using Core;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BootingSceneController : SceneController
{
    private void OnEnable()
    {
        GameDataManager.Instance.OnBoardDataUpdated += HandleBoardDataUpdated;
    }

    private void OnDisable()
    {
        GameDataManager.Instance.OnBoardDataUpdated -= HandleBoardDataUpdated;
    }

    void Start()
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
        Timing.RunCoroutine(ChangeScene());
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForSeconds(1f); 
        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_GAME);
    }
}
