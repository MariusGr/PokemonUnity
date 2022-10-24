using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterControllerBase : MonoBehaviour, ICharacterController
{
    [SerializeField] protected Character character;

    public abstract CharacterData CharacterData { get; }

    /**
     * Called by parent Character object of this controller to prevent issues with e.g. uninitialized Pokemon list etc.
     * **/
    virtual public void Init() { }

    protected float horizontal = 0;
    protected float vertical = 0;
    protected bool horizontalChanged = false;
    protected bool verticalChanged = false;
    protected bool sprinting = false;
}