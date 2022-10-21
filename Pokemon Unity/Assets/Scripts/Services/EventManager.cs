using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour
{
    public delegate void PauseEventHandler();
    public event PauseEventHandler PauseEvent;
    public event PauseEventHandler UnpauseEvent;
    public static EventManager Instance { get; private set; }

    private void Awake() => Instance = this;
    public static void Pause() => Instance.PauseEvent?.Invoke();
    public static void Unpause() => Instance.UnpauseEvent?.Invoke();
    public static void UnpauseDelayed(Coroutine coroutine) => Instance.StartCoroutine(Instance.UnpauseDelayedCoroutine(coroutine));

    private IEnumerator UnpauseDelayedCoroutine(Coroutine coroutine)
    {
        yield return coroutine;
        Unpause();
    }
}
