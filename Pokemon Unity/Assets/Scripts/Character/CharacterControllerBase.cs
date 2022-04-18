using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterControllerBase : MonoBehaviour
{
    [SerializeField] protected Character character;

    public CharacterData characterData;

    protected float horizontal = 0;
    protected float vertical = 0;
    protected bool horizontalChanged = false;
    protected bool verticalChanged = false;
    protected bool sprinting = false;
}
