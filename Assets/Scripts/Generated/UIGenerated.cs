using Core;

public struct UIName
{
	public const int IN_GAME_PANEL = -26392309;

	public const int SPLASH_SCREEN_TRANSITION = 686678414;

}

public class UIRegistration
{
	[UnityEngine.RuntimeInitializeOnLoadMethod]
	static void AssignUI()
	{
		UIHandler.AddView (-26392309, "InGamePanel", typeof(InGamePanel), "Assets/Prefabs/UI/Panel/InGamePanel.prefab", "Assets/Panel/InGamePanel", UILayer.Panel);

		UIHandler.AddView (686678414, "SplashScreenTransition", typeof(SplashScreenTransition), "Assets/Prefabs/UI/ScreenTransition/SplashScreenTransition.prefab", "Assets/ScreenTransition/SplashScreenTransition", UILayer.ScreenTransition);

	}
}