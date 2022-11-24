using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummarySelection : ScalarSelection
{
    public static SummarySelection Instance;

    [SerializeField] private SummaryUI ui;

    private const int MovesView = 3;
    private int pokemonIndex;
    private Pokemon pokemon => PlayerData.Instance.pokemons[pokemonIndex];
    private MoveButtonSummary swapMoveButton;

    public SummarySelection() => Instance = this;

    public void Open(int pokemonIndex)
    {
        SetPokemon(pokemonIndex);
        Open();
    }

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

    public override void Close()
    {
        ui.Close();
        base.Close();
    }

    private void SetPokemon(int index)
    {
        this.pokemonIndex = index;
        print(index);
        print(PlayerData.Instance.pokemons.Length);
        ui.AssignElement(pokemon);
    }

    protected override void ChooseSelectedElement()
    {
        if (selectedIndex == MovesView)
            ui.OpenMoveSelection(ChooseMove);
        else if (selectedIndex == 5)
            Close();
    }

    private void ChooseMove(ISelectableUIElement selection, bool goBack)
    {
        if(goBack)
        {
            ui.CloseMoveSelection();
            return;
        }

        MoveButtonSummary button = (MoveButtonSummary)selection;

        if (swapMoveButton is null)
        {
            button.SelectForSwap();
            swapMoveButton = button;
        }
        else
        {
            pokemon.SwapMoves(swapMoveButton.move, button.move);
            ui.RefreshMoves(pokemon);
            swapMoveButton.DeselectForSwap();
            swapMoveButton = null;
        }
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
