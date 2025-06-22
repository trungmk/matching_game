using Cysharp.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class NetworkClient : MonoSingleton<NetworkClient>
{
    [SerializeField]
    private string _socketUri = "wss://wss-exam-873220833062.us-central1.run.app/ws/board/{session-id}";

    private WebSocket _websocket;

    private IWebSocketHandler _webSocketHandler = null;

    private string _sessionId = string.Empty;

    public event System.Action OnConnectionSuccess;

    public void SetSessionId(string sessionId)
    {
        _sessionId = sessionId;
    }

    public void SubscribeWebSocketHandler(IWebSocketHandler webSocketHandler)
    {
        _webSocketHandler = webSocketHandler;
    }

    public void UnsubscribeWebSocketHandler()
    {
        _webSocketHandler = null;
    }

    public async void ConnectWebSocket()
    {
        if (string.IsNullOrEmpty(_sessionId))
        {
            Debug.LogError("Session ID is not set.");
            return;
        }

        string finalUri = _socketUri.Replace("{session-id}", _sessionId);
        _websocket = new WebSocket(finalUri);

        try
        {
            _websocket.OnOpen += () =>
            {
                if (_webSocketHandler != null)
                {
                    _webSocketHandler.HandleOpen();
                }
            };

            _websocket.OnError += (e) =>
            {
                Debug.LogError("WebSocket Error: " + e);

                if (_webSocketHandler != null)
                {
                    _webSocketHandler.HandleError(e);
                }
            };

            _websocket.OnClose += (e) =>
            {
                Debug.LogWarning("Connection closed!");

                if (_webSocketHandler != null)
                {
                    _webSocketHandler.HandleClose(e);
                }
            };

            _websocket.OnMessage += (bytes) =>
            {
                if (_webSocketHandler != null && bytes != null && bytes.Length > 0)
                {
                    _webSocketHandler.handleMessage(bytes);
                }
            };

            await _websocket.Connect();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to initialize WebSocket: " + ex.Message);
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        if (_websocket != null)
        {
            try
            {
                UnsubscribeWebSocketHandler();
                await _websocket.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to close WebSocket: " + ex.Message);
            }
        }
    }

    public async UniTask SendSocketMessage(string message)
    {
        if (_websocket != null && _websocket.State == WebSocketState.Open)
        {
            await _websocket.SendText(message);
        }
        else
        {
            Debug.LogWarning("WebSocket is not open. Message not sent.");
        }
    }

    public async void Send(byte[] bytesMessage)
    {
        if (_websocket != null && _websocket.State == WebSocketState.Open)
        {
            await _websocket.Send(bytesMessage);
        }
        else
        {
            Debug.LogWarning("WebSocket is not open. Message not sent.");
        }
    }
}