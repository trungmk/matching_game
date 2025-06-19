using Core;

public class StartGameEvent : IEvent
{
    private static readonly StartGameEvent _instance = new StartGameEvent();

    public static StartGameEvent GetInstance()
    {
        return _instance;
    }

    public void Reset()
    {
    }
}
