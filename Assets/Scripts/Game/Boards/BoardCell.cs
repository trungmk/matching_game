using UnityEngine;

public class BoardCell 
{
    public Vector2Int Position { get; set; }

    public Tile Tile { get; set; }

    public bool IsEnable { get; set; }

    public bool IsBlocker { get; set; }
}