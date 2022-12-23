using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveGameManager : IService
{
    public void Register(ISavable savable);
}
