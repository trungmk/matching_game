using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameDataManager : MonoSingleton<GameDataManager>
{
    public event Action<BoardData> OnBoardDataUpdated;

    private BoardData _currentBoardData;

    public BoardData CurrentBoardData => _currentBoardData;

    public BoardData RemoteData { get; set; }

    public BoardData LocalData { get; set; }

    public bool IsUseRemoteData { get; set; } = false;

    private void OnEnable()
    {
        WebSocketHandler.Instance.OnGetMessageSuccess += UpdateBoardMessage;
    }

    private void OnDisable()
    {
        WebSocketHandler.Instance.OnGetMessageSuccess -= UpdateBoardMessage;
    }

    public void UpdateBoardMessage(BoardMessage boardMessage)
    {
        if (boardMessage == null || boardMessage.Board == null) 
            return;

        RemoteData = boardMessage.Board;
        OnBoardDataUpdated?.Invoke(RemoteData);
    }

    public void UpdateBoardData(BoardData boardData, bool isRemote = true)
    {
        if (boardData == null)
        {
            return;
        }

        _currentBoardData = boardData;
    }
    
    public void ClearBoardData()
    {
        _currentBoardData = null;
    }
}