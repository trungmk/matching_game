using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LoadDataEvent : IEvent
{
    private static readonly LoadDataEvent _instance = new LoadDataEvent();

    public bool IsUseRemoteData { get; set; } = false;

    public static LoadDataEvent GetInstance(bool isUseSocketData)
    {
        _instance.IsUseRemoteData = isUseSocketData;
        return _instance;
    }

    public void Reset()
    {
        _instance.IsUseRemoteData = false;
    }
}