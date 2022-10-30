using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSwitchSelection : SelectionWindow
{
    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        ProcessInput();
    }

    protected override void ChooseSelectedElement()
    {
        print("Choose Pkmn");
        Services.Get<IBattleManager>().ChoosePlayerPokemon((selectedElement).index);
    }
}
