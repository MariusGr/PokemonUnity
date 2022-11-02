using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerPlayer : CharacterControllerBase, IInputConsumer
{
    public PlayerData characterData;
    override public CharacterData CharacterData => characterData;

    InputData input = new InputData();

    private void Start()
    {
        InputManager.Instance.Register(this);
    }

    private void Update()
    {
        character.Movement.ProcessMovement(input.digitalPad.heldDown, input.chancel.heldDown);
    }

    public bool ProcessInput(InputData input)
    {
        this.input = input;

        if (input.submit.pressed)
            character.TryInteract();

        return false;
    }
}
