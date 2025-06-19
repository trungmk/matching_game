using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private BoardMono _board;

    public void InitializeBoard(BoardData boardData)
    {
        if (_board == null)
        {
            Debug.LogError("BoardMono is not assigned in the inspector.");
            return;
        }

        _board.Initialize(boardData);
    }
}
