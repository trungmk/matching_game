public struct ContextNameGenerated
{
	public const int CONTEXT_BOOTING = -81938762;

	public const int CONTEXT_GAME = 1582856036;

	public const int CONTEXT_MENU = -1078770617;

}

public static class ContextRegistration
{
	[UnityEngine.RuntimeInitializeOnLoadMethod]
	static void AssignContext()
	{
		SceneHandler.AddContext (-81938762, "Booting", "Assets/Scenes/Match_3/Booting/Booting.unity");

		SceneHandler.AddContext (1582856036, "Game", "Assets/Scenes/Match_3/Game/Game.unity");

		SceneHandler.AddContext (-1078770617, "Menu", "Assets/Scenes/Match_3/Menu/Menu.unity");

	}
}