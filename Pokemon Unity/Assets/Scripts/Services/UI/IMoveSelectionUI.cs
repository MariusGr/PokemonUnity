using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveSelectionUI : IUIView
{
    public void Assign(Pokemon pokemon);
    public void RefreshMove(Move move);
}
