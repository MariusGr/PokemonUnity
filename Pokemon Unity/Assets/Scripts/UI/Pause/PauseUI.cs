using UnityEngine;

public class PauseUI : SelectionGraphWindow, IPauseUI
{
    [SerializeField] PokedexSelection dexView;
    [SerializeField] PartySelection partyView;
    [SerializeField] BagUI bagView;
    [SerializeField] SaveGameUI saveGameView;

    public PauseUI() => Services.Register(this as IPauseUI);

    private void Awake()
    {
        AssignElements(new object[] { null, null, null, null });
    }

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

    private void ChooseOption(int index)
    {
        if (index == 0)
            dexView.Open(callback: CloseDexView);
        else if (index == 1)
            partyView.Open(callback: ClosePartyView, battle: false, forceSelection: false);
        else if (index == 2)
            bagView.Open(callback: CloseBagView);
        else if (index == 3)
            saveGameView.Open(CloseSaveGameView);
    }

    private void CloseDexView(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
            dexView.Close();
    }

    private void CloseBagView(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
            bagView.Close();
    }

    private void ClosePartyView(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
            partyView.Close();
    }

    private void CloseSaveGameView() => saveGameView.Close();

    protected override void GoBack()
    {
        base.GoBack();
        Close();
    }

    public void RefreshMove(Move move) => RefreshElement(move.Index);
}
