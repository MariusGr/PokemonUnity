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
        horizontalChanged = Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal");
        verticalChanged = Input.GetButtonDown("Vertical") || Input.GetButtonUp("Vertical");

        character.Movement.ProcessMovement(horizontal, vertical, horizontalChanged, verticalChanged, sprinting);
    }
}
