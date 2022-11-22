using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummarySelection : ScalarSelection
{
    public static SummarySelection Instance;

    [SerializeField] private SummaryUI ui;

    private int pokemonIndex;
    private Pokemon pokemon => PlayerData.Instance.pokemons[pokemonIndex];

    public SummarySelection() => Instance = this;

    public override void Open(Action<ISelectableUIElement, bool> callback, int pokemonIndex)
    {
        SetPokemon(pokemonIndex);
        Open(callback);
    }

    public override void Open(Action<ISelectableUIElement, bool> callback)
    {
        ui.Open();
        base.Open(callback);
    }

    private void SetPokemon(int index)
    {
        this.pokemonIndex = index;
        print(index);
        print(PlayerData.Instance.pokemons.Length);
        ui.AssignElement(pokemon);
    }

    public override bool ProcessInput(InputData input)
    {
        if(!base.ProcessInput(input))
        {
            if (input.digitalPad.pressed == Direction.Up)
            {
                SetPokemon((pokemonIndex + 1) % PlayerData.Instance.pokemons.Length);
                return true;
            }
            if (input.digitalPad.pressed == Direction.Down)
            {
                SetPokemon(pokemonIndex < 1 ? PlayerData.Instance.pokemons.Length - 1 : pokemonIndex - 1);
                return true;
            }
        }
        return false;
    }
}
