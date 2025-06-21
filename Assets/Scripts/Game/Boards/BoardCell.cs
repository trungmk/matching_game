using UnityEngine;

public class BoardCell 
{
    public Vector2 LocalPosition { get; set; }

    public BaseTile Tile { get; set; }

    public bool IsEnable { get; set; }

    public bool IsBlocker { get; set; }
}