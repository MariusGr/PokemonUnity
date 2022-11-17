using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pausable : MonoBehaviour
{
    protected bool paused = false;

    protected void SignUpForPause()
    {
        EventManager.Instance.PauseEvent += () => paused = true;
        EventManager.Instance.UnpauseEvent += () => paused = false;
    }
}
