using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerCharacter : IService
{
    public Coroutine Defeat();
}
