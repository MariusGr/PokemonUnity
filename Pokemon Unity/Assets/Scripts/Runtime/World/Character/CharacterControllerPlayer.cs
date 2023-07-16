using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerPlayer : CharacterControllerBase, IInputConsumer
{
    [SerializeField] public PlayerData playerData;
    override public CharacterData CharacterData => playerData;

    private InputData input = new InputData();
    private Door currentEntrance = null;

    private void Start()
    {
        InputManager.Instance.Register(this);
        DebugExtensions.DebugExtension.Log(playerData.pokemons);
        SignUpForPause();
    }

    private void Update()
    {
        if (paused)
            return;

        if (!character.Movement.moving && !(currentEntrance is null) && input.digitalPad.heldDown == currentEntrance.directionTriggerToEntrance)
        {
            character.Movement.LookInDirection(currentEntrance.directionTriggerToEntrance);
            currentEntrance.Enter();
        }
        else
            character.Movement.ProcessMovement(input.digitalPad.heldDown, input.chancel.heldDown);
    }

    public bool ProcessInput(InputData input)
    {
        if (paused)
            return false;

        this.input = input;

        if (input.submit.pressed && !character.Movement.moving)
            character.TryInteract();
        else if (input.start.pressed && !character.Movement.moving)
            PauseUI.Instance.Open();

        return false;
    }

    public void EnterEntranceTrehshold(Door entrance)
    {
        currentEntrance = entrance;
    }

    public void LeaveEntranceTrehshold(Door entrance)
    {
        if (currentEntrance == entrance)
            currentEntrance = null;
    }
}