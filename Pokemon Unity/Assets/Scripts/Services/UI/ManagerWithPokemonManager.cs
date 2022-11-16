using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerWithPokemonManager : ManagerWithDialogBox
{
    protected PokemonManager pokemonManager;

    protected override void Initialize()
    {
        base.Initialize();
        pokemonManager = (PokemonManager)Services.Get<IPokemonManager>();
    }
}
