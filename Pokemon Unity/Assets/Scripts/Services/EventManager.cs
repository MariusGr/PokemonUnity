public class EventManager
{
    public delegate void PauseEventHandler();
    public event PauseEventHandler PauseEvent;
    public event PauseEventHandler UnpauseEvent;
    public event PauseEventHandler CheckNPCVisionEvent;

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
    public static void CheckNPCVision() => instance.CheckNPCVisionEvent?.Invoke();
}
