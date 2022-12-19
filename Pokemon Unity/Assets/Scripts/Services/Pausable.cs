using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pausable : MonoBehaviour
{
    protected bool paused = false;

    protected virtual void Pause() => paused = true;
    protected virtual void Unpause() => paused = false;

    protected void SignUpForPause()
    {
        EventManager.Instance.PauseEvent += Pause;
        EventManager.Instance.UnpauseEvent += Unpause;
    }
}
