using Newtonsoft.Json;
using System;

[Serializable]
public class BoardMessage
{
    [JsonProperty("type")]
    public string Type;

    [JsonProperty("board")]
    public BoardData Board;

    [JsonProperty("timestamp")]
    public long TimeStamp;
}