public class EventManager
{
    public delegate void PauseEventHandler();
    public event PauseEventHandler PauseEvent;
    public event PauseEventHandler UnpauseEvent;

    static EventManager instance;

    public static EventManager Instance
    {
        get
        {
            if (instance == null)
                instance = new EventManager();
            return instance;
        }
    }

    public static void Pause() => instance.PauseEvent?.Invoke();
    public static void Unpause() => instance.UnpauseEvent?.Invoke();
}
