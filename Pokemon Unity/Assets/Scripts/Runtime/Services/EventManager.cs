using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour
{
    public delegate void PauseEventHandler();
    public event PauseEventHandler PauseEvent;
    public event PauseEventHandler UnpauseEvent;
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Screen.SetResolution(2960, 1440, FullScreenMode.FullScreenWindow);
    }

    public static void Pause() => Instance.PauseEvent?.Invoke();
    public static void Unpause() => Instance.UnpauseEvent?.Invoke();
    public static void UnpauseDelayed(Coroutine coroutine) => Instance.StartCoroutine(Instance.UnpauseDelayedCoroutine(coroutine));

    private IEnumerator UnpauseDelayedCoroutine(Coroutine coroutine)
    {
        yield return coroutine;
        Unpause();
    }

    public Coroutine PerformCoroutines(List<IEnumerator> routines, Action onRoutinesFinished = null)
        => StartCoroutine(PerformCoroutinesRoutine(routines, onRoutinesFinished));
    private IEnumerator PerformCoroutinesRoutine(List<IEnumerator> routines, Action onRoutinesFinished = null)
    {
        foreach (var routine in routines)
            yield return StartCoroutine(routine);
        onRoutinesFinished?.Invoke();
    }
}
