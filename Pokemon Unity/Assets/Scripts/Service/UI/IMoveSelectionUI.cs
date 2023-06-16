using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveSelectionUI : ISelectionWindow
{
    public void Assign(IPokemon pokemon);
    public void RefreshMove(IMove move);
}
