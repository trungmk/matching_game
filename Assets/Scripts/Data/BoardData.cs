using Core;
using Newtonsoft.Json;
using ProtoBuf;

[System.Serializable]
public class BoardData : ILocalData
{
    [JsonProperty("items")]
    public BoardItem[][] Items;

    [JsonProperty("size")]
    public int Size;

    public void InitAfterLoadData()
    {

    }
}
