using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionUI : SelectionGraphWindow
{
    public void Assign(Pokemon pokemon) => AssignElements(pokemon.moves.ToArray());
    public void RefreshMove(Move move) => RefreshElement(move.index);
}
