using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LocalDataManager : LocalDataManagerBase
{
    protected override void ConfigureLocalData()
    {
        AssignLocalData<BoardDataProto>(new LocalDataProtoBufWrapper<BoardDataProto>());
    }
}