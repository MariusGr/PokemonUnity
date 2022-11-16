using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIView : IService
{
    public void Open();
    public void Close();
}
