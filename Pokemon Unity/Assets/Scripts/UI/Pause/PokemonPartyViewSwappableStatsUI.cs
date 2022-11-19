using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonPartyViewSwappableStatsUI : PokemonPartyViewStatsUI
{
    [SerializeField] private Sprite backgroundSwap;
    [SerializeField] private Sprite backgroundSwapSelected;

    protected override Sprite GetCurrentBackgroundIdle() => swap ? backgroundSwap : base.GetCurrentBackgroundIdle();
    protected override Sprite GetCurrentBackgroundSelected() => swap ? backgroundSwapSelected : base.GetCurrentBackgroundSelected();
    private bool swap = false;

    private void Awake() => DoOnAwake();
    private void OnEnable() => DoOnEnable();

    public void Refresh(bool swap)
    {
        this.swap = swap;
        base.Refresh();
    }
}
