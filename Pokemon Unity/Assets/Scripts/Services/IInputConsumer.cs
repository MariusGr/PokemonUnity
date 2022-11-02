using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputConsumer
{
    public bool ProcessInput(InputData input);
}
