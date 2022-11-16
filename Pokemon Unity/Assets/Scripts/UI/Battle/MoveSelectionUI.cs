using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : SelectionGraphWindow, IMoveSelectionUI
{
    public MoveSelectionUI() => Services.Register(this as IMoveSelectionUI);

    protected override void ChooseSelectedElement()
    {
        base.ChooseSelectedElement();
        print("Choose Move");
        Services.Get<IPokemonManager>().ChoosePlayerMove(((MoveButton)selectedElement).move, false);
    }

    protected override void GoBack()
    {
        base.GoBack();
        print("back");
        Services.Get<IPokemonManager>().ChoosePlayerMove(null, true);
    }

    public void Assign(Pokemon pokemon) => AssignElements(pokemon.moves.ToArray());
    public void RefreshMove(Move move) => RefreshElement(move.index);
}
