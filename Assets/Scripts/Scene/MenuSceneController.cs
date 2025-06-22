using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MenuSceneController : SceneController
{
    private void Handle_LoadDataEvent(LoadDataEvent @event)
    {
        GameDataManager.Instance.IsUseRemoteData = @event.IsUseRemoteData;

        UIManager.Instance.Show<ScreenTransition>()
            .OnShowCompleted(v =>
            {
                CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_GAME);
            });

    }

    public override void OnLoaded()
    {
        EventManager.Instance.AddListener<LoadDataEvent>(Handle_LoadDataEvent);

        UIManager.Instance.Show<MenuPanel>()
            .OnShowCompleted(v =>
            {
                UIManager.Instance.Hide<SplashScreenTransition>(isDisable: true, isDestroy: true);
            });
    }

    public override void OnUnloaded()
    {
        EventManager.Instance.RemoveListener<LoadDataEvent>(Handle_LoadDataEvent);
        UIManager.Instance.Hide<MenuPanel>();
    }
}
