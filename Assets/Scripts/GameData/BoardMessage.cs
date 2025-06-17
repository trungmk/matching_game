using System;

[Serializable]
public class BoardMessage {
    public string type;
    public Board board;
    public long timestamp;
}
