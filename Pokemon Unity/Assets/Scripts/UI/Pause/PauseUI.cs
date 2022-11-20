using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : SelectionGraphWindow, IPauseUI
{
    [SerializeField] PokemonSwitchSelection partyView;

    public PauseUI() => Services.Register(this as IPauseUI);

    public override void Open()
    {
        Open((ISelectableUIElement selection, bool goBack) =>
        {
            ChooseOption(selection.GetIndex());
        });
    }

    private  void ChooseOption(int index)
    {
        if (index == 0)
            partyView.Open();
    }

    protected override void GoBack()
    {
        base.GoBack();
        Close();
    }

    public void Assign(CharacterData player) => partyView.AssignElements(player.pokemons);
    public void RefreshMove(Move move) => RefreshElement(move.index);
}
