using Core;
using UnityEngine;

public class GameSceneController : SceneController
{
    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private LevelManager _levelManager;

    public override void OnLoaded()
    {
        UIManager.Instance.Show<InGamePanel>();

        //_levelManager.LoadLevel(1);

        _gameManager.InitGame(() =>
        {
            UIManager.Instance.Hide<SplashScreenTransition>(isDisable: true, isDestroy: true);
        });
    }
}
