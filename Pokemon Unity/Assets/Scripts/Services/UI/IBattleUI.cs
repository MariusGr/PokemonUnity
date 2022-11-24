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
    public System.Func<bool> RefreshHPAnimated(int character);
    public System.Func<bool> RefreshXPAnimated();
    public System.Func<bool> PlayMoveAnimation(int attacker, Move move);
    public System.Func<bool> PlayBlinkAnimation(int blinkingPokemon);
    public System.Func<bool> PlayFaintAnimation(int faintedOwner);
    public System.Func<bool> MakeOpponentAppear();
    public System.Func<bool> MakeOpponentDisappear();
    public void OpenBattleMenu(System.Action<BattleOption, bool> callback);
    public void OpenPokemonSwitchSelection(System.Action<ISelectableUIElement, bool> callback, bool forceSelection = false);
    public void OpenMoveSelection(System.Action<ISelectableUIElement, bool> callback, Pokemon pokemon);
    public void CloseBattleMenu();
    public void ClosePokemonSwitchSelection();
    public void CloseMoveSelection();
}
