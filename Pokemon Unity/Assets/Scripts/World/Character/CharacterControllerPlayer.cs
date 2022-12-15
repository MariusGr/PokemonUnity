using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerPlayer : CharacterControllerBase, IInputConsumer
{
    public PlayerData characterData;
    override public CharacterData CharacterData => characterData;

    InputData input = new InputData();
    private IPauseUI pauseUI;

    private void Awake()
    {
#if (UNITY_EDITOR)
        characterData.FillItemsDict();
#endif
        foreach (Pokemon p in characterData.pokemons)
            characterData.AddCaughtPokemon(p.data);
        foreach (Pokemon p in characterData.pokemonsInBox)
            characterData.AddCaughtPokemon(p.data);
    }

    private void Start()
    {
        InputManager.Instance.Register(this);
        pauseUI = Services.Get<IPauseUI>();
        DebugExtensions.DebugExtension.Log(characterData.pokemons);
        SignUpForPause();
    }

    private void Update()
    {
        character.Movement.ProcessMovement(input.digitalPad.heldDown, input.chancel.heldDown);
    }

    public bool ProcessInput(InputData input)
    {
        if (paused)
            return false;

        this.input = input;

        if (input.submit.pressed)
            character.TryInteract();
        else if (input.start.pressed)
            pauseUI.Open();

        return false;
    }
}
