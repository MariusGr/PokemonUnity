using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterControllerBase : MonoBehaviour
{
    [SerializeField] protected Character character;

    protected float horizontal = 0;
    protected float vertical = 0;
    protected bool sprinting = false;
}
