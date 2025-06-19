using Newtonsoft.Json;
using System;

[Serializable]
public class BoardItem
{
    [JsonProperty("id")]
    public int Id;

    [JsonProperty("type")]
    public string Type;

    [JsonProperty("x")]
    public int X;

    [JsonProperty("y")]
    public int Y;
}