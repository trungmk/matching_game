using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ProtoContract]
public class BoardItemProto
{
    [ProtoMember(1)]
    public int Id;

    [ProtoMember(2)]
    public string Type;

    [ProtoMember(3)]
    public int X;

    [ProtoMember(4)]
    public int Y;
}