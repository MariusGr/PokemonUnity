using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenedInputConsumer : MonoBehaviour, IInputConsumer
{
    public void Open()
    {
        gameObject.SetActive(true);
        InputManager.Instance.Register(this);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        InputManager.Instance.Unregister(this);
    }

    public virtual bool ProcessInput(InputData input) => false;
}
