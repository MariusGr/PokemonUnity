using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSwitchSelection : SelectionGraphWindow
{
    protected override void ChooseSelectedElement() => Services.Get<IBattleManager>().ChoosePlayerPokemon((selectedElement).index, false);
    protected override void GoBack() => Services.Get<IBattleManager>().ChoosePlayerPokemon(-1, true);
}
