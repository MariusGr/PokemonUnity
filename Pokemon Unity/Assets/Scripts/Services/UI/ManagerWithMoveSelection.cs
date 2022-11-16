using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerWithMoveSelection : ManagerWithPokemonManager
{
    protected IMoveSelectionUI moveSelectionUI;

    protected override void Initialize()
    {
        base.Initialize();
        moveSelectionUI = Services.Get<IMoveSelectionUI>();
    }
}
