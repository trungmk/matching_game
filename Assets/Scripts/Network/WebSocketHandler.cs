using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

public class WebSocketHandler : IWebSocketHandler
{
    public void HandleClose(WebSocketCloseCode webSocketCloseCode)
    {
        // handle close event from the server
    }

    public void HandleError(string errorMessage)
    {
        // handle error message from the server
    }

    public void handleMessage(byte[] messageBytes)
    {
        try
        {
            string data = System.Text.Encoding.UTF8.GetString(messageBytes);
            BoardData boardData = JsonConvert.DeserializeObject<BoardData>(data);

            if (boardData == null || string.IsNullOrEmpty(boardData.Type))
            {
                Debug.LogError("Invalid board data or type");
                return;
            }

            if (boardData.Type == "board")
            {

            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error handling message: " + ex.Message);
            return;
        }
    }

    public void HandleOpen()
    {
        // handle close event from the server
    }
}
