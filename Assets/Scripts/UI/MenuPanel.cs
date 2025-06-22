using Core;
using UnityEngine;

public class MenuPanel : PanelView
{
    public void OnLoadLocalData_ButtonClicked()
    {
        EventManager.Instance.Dispatch(LoadDataEvent.GetInstance(isUseSocketData: false));
    }

    public void OnLoadRemoteData_ButtonClicked()
    {
        EventManager.Instance.Dispatch(LoadDataEvent.GetInstance(isUseSocketData: true));
    }
}
