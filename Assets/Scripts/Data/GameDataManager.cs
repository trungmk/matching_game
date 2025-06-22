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

    private BoardData _boardData;

    public BoardData CurrentBoardData => _boardData;

    public BoardData GetBoardData()
    {
        return _boardData;
    }

    public async UniTask<BoardData> GetBoardDataAsync()
    {
        if (_boardData == null)
        {
            await UniTask.Yield();
        }
        return _boardData;
    }

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
        
        _boardData = boardMessage.Board;
        OnBoardDataUpdated?.Invoke(_boardData);
    }

    public void UpdateBoardData(BoardData boardData, bool isRemote = true)
    {
        if (boardData == null)
        {
            return;
        }

        _boardData = boardData;
    }
    
    public void ClearBoardData()
    {
        _boardData = null;
    }
}