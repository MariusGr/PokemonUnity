using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSwitchSelection : SelectionGraphWindow
{
    private void Update() => ProcessInput();
    protected override void ChooseSelectedElement() => Services.Get<IBattleManager>().ChoosePlayerPokemon((selectedElement).index);
}
