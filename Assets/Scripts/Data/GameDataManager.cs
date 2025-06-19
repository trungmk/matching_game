using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GameDataManager : MonoSingleton<GameDataManager>
{
    public event Action<BoardData> OnBoardDataUpdated;

    public BoardData CurrentBoardData { get; private set; }

    private void OnEnable()
    {
        WebSocketHandler.Instance.OnGetMessageSuccess += UpdateBoardData;
    }

    private void OnDisable()
    {
        WebSocketHandler.Instance.OnGetMessageSuccess -= UpdateBoardData;
    }

    public void UpdateBoardData(BoardMessage boardMessage)
    {
        if (boardMessage == null || boardMessage.Board == null) 
            return;
        
        CurrentBoardData = boardMessage.Board;
        OnBoardDataUpdated?.Invoke(CurrentBoardData);
    }
    
    public void ClearBoardData()
    {
        CurrentBoardData = null;
    }
}