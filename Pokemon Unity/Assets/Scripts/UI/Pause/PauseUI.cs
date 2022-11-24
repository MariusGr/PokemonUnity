using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : SelectionGraphWindow, IPauseUI
{
    [SerializeField] PartySelection partyView;

    public PauseUI() => Services.Register(this as IPauseUI);

    public override void Open()
    {
        Open((ISelectableUIElement selection, bool goBack) =>
        {
            if (goBack || selection is null)
                Close();
            else
                ChooseOption(selection.GetIndex());
        });
    }

    override public bool ProcessInput(InputData input)
    {
        if (!base.ProcessInput(input))
            if (input.start.pressed)
            {
                Close();
                return true;
            }

        return false;
    }

    private  void ChooseOption(int index)
    {
        if (index == 0)
            partyView.Open(ClosePartyView);
    }

    private void ClosePartyView(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
            partyView.Close();
    }

    protected override void GoBack()
    {
        base.GoBack();
        Close();
    }

    public void Assign(CharacterData player) => partyView.AssignElements(player.pokemons);
    public void RefreshMove(Move move) => RefreshElement(move.index);
}
