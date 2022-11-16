using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIView : MonoBehaviour, IUIView
{
    public virtual void Open() => gameObject.SetActive(true);
    public virtual void Close() => gameObject.SetActive(false);
}
