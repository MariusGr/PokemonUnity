using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerPlayer : CharacterControllerBase
{
    void Start()
    {
        
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        sprinting = Input.GetButton("Sprint");

        character.Movement.ProcessMovement(horizontal, vertical, sprinting);
    }
}
