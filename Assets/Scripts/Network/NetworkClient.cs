using System;
using WebSocketSharp;
using UnityEngine;
using System.Threading;

public class NetworkClient : MonoSingleton<NetworkClient>
{
    [SerializeField]
    private string _socketUri = "wss://wss-exam-873220833062.us-central1.run.app";

    private WebSocket _socket;

    private void Start()
    {
        ConnectWebSocket();
    }

    private void ConnectWebSocket()
    {
        _socket = new WebSocket(_socketUri);
        _socket.Connect();

        _socket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened.");
        };

        _socket.OnMessage += (sender, e) =>
        {
            Debug.Log($"Message received: {e.Data}");
        };

        _socket.OnError += (sender, e) =>
        {
            Debug.LogError($"WebSocket error: {e.Message}");
        };

        _socket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed.");
        };

        _socket.Connect();
    }

    public void SendMessage(string message)
    {
        if (_socket != null && _socket.IsAlive)
        {
            _socket.Send(message);
        }
    }

    private void OnDestroy()
    {
        if (_socket != null)
        {
            _socket.Close();
            _socket = null;
        }
    }
}
