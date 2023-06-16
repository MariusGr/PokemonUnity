using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerWithPokemonManager : ManagerWithDialogBox
{
    protected IPokemonManager pokemonManager;

    protected override void Initialize()
    {
        base.Initialize();
        pokemonManager = (IPokemonManager)Services.Get<IPokemonManager>();
    }
}
