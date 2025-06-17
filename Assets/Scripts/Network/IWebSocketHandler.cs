using NativeWebSocket;
using UnityEngine;

public interface IWebSocketHandler
{
    void HandleOpen();

    void HandleError(string errorMessage);

    void HandleClose(WebSocketCloseCode webSocketCloseCode);

    void handleMessage(byte[] messageBytes);
}
