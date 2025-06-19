using Newtonsoft.Json;

[System.Serializable]
public class BoardData
{
    //public IList<IList<BoardItem>> Items;
    [JsonProperty("items")]
    public BoardItem[][] Items;

    [JsonProperty("size")]
    public int Size;
}
