using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleUI : IUIView
{
    public void Open(CharacterData playerData, Pokemon playerPokemon, Pokemon opponentPokemon);
    public void SwitchToPokemon(int characterIndex, Pokemon pokemon);
    public void RefreshPlayerStats(bool refreshXP = false);
    public void Refresh(int character);
    public void RefreshHP(int character);
    public void RefreshXP();
    public void ResetXP();
    public void RefreshMove(Move move);
    public IEnumerator RefreshHPAnimated(int character);
    public IEnumerator RefreshXPAnimated();

    public IEnumerator PlayMoveAnimation(int attacker, Move move);
    public IEnumerator PlayBlinkAnimation(int blinkingPokemon);
    public IEnumerator PlayFaintAnimation(int faintedOwner);
    public IEnumerator PlayInflictStatusAnimation(int owner);
    public IEnumerator PlayStatUpAnimation(int owner);
    public IEnumerator PlayStatDownAnimation(int owner);

    public IEnumerator MakeOpponentAppear();
    public IEnumerator MakeOpponentDisappear();
    public void OpenBattleMenu(System.Action<BattleOption, bool> callback);
    public void OpenPokemonSwitchSelection(System.Action<ISelectableUIElement, bool> callback, bool forceSelection = false);
    public void OpenMoveSelection(System.Action<ISelectableUIElement, bool> callback, Pokemon pokemon);
    public void OpenBagSelection(System.Action<ISelectableUIElement, bool> callback);
    public void CloseBattleMenu();
    public void ClosePokemonSwitchSelection();
    public void CloseMoveSelection();
    public void CloseBagSelection();
}
