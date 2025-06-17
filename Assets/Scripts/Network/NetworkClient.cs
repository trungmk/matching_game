using NativeWebSocket;
using UnityEngine;

public class NetworkClient : MonoSingleton<NetworkClient>
{
    [SerializeField]
    private string _socketUri = "wss://wss-exam-873220833062.us-central1.run.app/ws/board/{session-id}";

    private WebSocket _websocket;

    private bool _isInitialized = false;

    private IWebSocketHandler _webSocketHandler = null;

    public void SubscribeWebSocketHandler(IWebSocketHandler webSocketHandler)
    {
        _webSocketHandler = webSocketHandler;
    }

    public void UnsubscribeWebSocketHandler()
    {
        _webSocketHandler = null;
    }

    // Start is called before the first frame update
    public async void ConnectWebSocket()
    {
        try
        {
            _websocket = new WebSocket(_socketUri);

            _websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");

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
                Debug.Log("Message received!");
                Debug.Log("Bytes: " + System.BitConverter.ToString(bytes));

                if (_webSocketHandler != null && bytes != null && bytes.Length > 0)
                {
                    _webSocketHandler.handleMessage(bytes);
                }
            };

            await _websocket.Connect();

            InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 0.3f);

            _isInitialized = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to initialize WebSocket: " + ex.Message);
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (!_isInitialized)
            return;
        
        _websocket?.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (_websocket != null && _websocket.State == WebSocketState.Open)
        {
            try
            {

                // Sending bytes
                await _websocket.Send(new byte[] { 10, 20, 30 });

                // Sending plain text
                await _websocket.SendText("plain text message");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to send WebSocket message: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("WebSocket is not open. Message not sent.");
        }
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

    public async void SendMessage(string message)
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