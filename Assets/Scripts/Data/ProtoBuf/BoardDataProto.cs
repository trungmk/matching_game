using Core;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ProtoContract]
public class BoardDataProto : ILocalData
{
    [ProtoMember(1)]
    public List<BoardItemProto> Items { get; set; } = new List<BoardItemProto>();

    [ProtoMember(2)]
    public int Size { get; set; }

    [ProtoMember(3)]
    public int RowLength { get; set; }

    public void ConvertBoardItem(BoardItem[][] boardItems)
    {
        Items.Clear();
        Size = boardItems.Length;
        RowLength = boardItems[0].Length;
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < RowLength; j++)
            {
                var item = boardItems[i][j];
                Items.Add(new BoardItemProto
                {
                    Id = item.Id,
                    Type = item.Type,
                    X = item.X,
                    Y = item.Y
                });
            }
        }
    }

    public void InitAfterLoadData()
    {

    }
}