using NativeWebSocket;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class WebSocketHandler : MonoSingleton<WebSocketHandler>, IWebSocketHandler
{
    [SerializeField]
    private NetworkClient _networkClient;

    public event Action OnConnectionSuccess;

    public event Action<BoardMessage> OnGetMessageSuccess;

    public event Action OnSocketClosed;

    public event Action OnSocketError;

    private void OnEnable()
    {
        _networkClient.SubscribeWebSocketHandler(this);
    }

    private void OnDisable()
    {
        _networkClient.UnsubscribeWebSocketHandler();
    }

    public void HandleClose(WebSocketCloseCode webSocketCloseCode)
    {
        // handle close event from the server
        if (OnSocketClosed != null)
        {
            OnSocketClosed();
        }
    }

    public void HandleError(string errorMessage)
    {
        // handle error message from the server
        if (OnSocketError != null)
        {
            OnSocketError();
        }
    }

    public void HandleOpen()
    {
        if (OnConnectionSuccess != null)
        {
            OnConnectionSuccess();
        }
    }

    public void handleMessage(byte[] messageBytes)
    {
        try
        {
            string data = System.Text.Encoding.UTF8.GetString(messageBytes);
            BoardMessage boardData = JsonConvert.DeserializeObject<BoardMessage>(data);

            if (boardData == null || string.IsNullOrEmpty(boardData.Type))
            {
                return;
            }

            if (string.Equals(boardData.Type, "board", System.StringComparison.OrdinalIgnoreCase))
            {
                OnGetMessageSuccess(boardData);
            }
            else
            {
                Debug.LogWarning("Unknown board data type: " + boardData.Type);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error handling message: " + ex.Message);
            return;
        }
    }

}
