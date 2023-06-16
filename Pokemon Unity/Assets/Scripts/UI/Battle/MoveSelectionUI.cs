using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : SelectionGraphWindow, IMoveSelectionUI
{
    public void Assign(IPokemon pokemon) => AssignElements(pokemon.Moves.ToArray());
    public void RefreshMove(IMove move) => RefreshElement(move.Index);
}
