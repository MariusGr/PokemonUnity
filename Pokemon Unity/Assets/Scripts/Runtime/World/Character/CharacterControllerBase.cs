using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterControllerBase : Pausable
{
    [field: SerializeField] public Character character { get; protected set; }
    protected new GameObject gameObject; // Only needed to override gameobject field of inherited Monobehaviour

    /**
     * Called by parent Character object of this controller to prevent issues with e.g. uninitialized Pokemon list etc.
     * **/
    virtual public void Initialize() { }
}
