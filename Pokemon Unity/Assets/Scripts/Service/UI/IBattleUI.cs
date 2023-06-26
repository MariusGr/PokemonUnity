using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleUI : IService
{
    public Coroutine Open(ICharacterData playerData, IPokemon playerPokemon, IPokemon opponentPokemon);
    public Coroutine Close();
    public void SwitchToPokemon(int characterIndex, IPokemon pokemon);
    public void RefreshPlayerStats(bool refreshXP = false);
    public void Refresh(int character);
    public void RefreshHP(int character);
    public void RefreshXP();
    public void ResetXP();
    public void RefreshMove(IMove move);
    public IEnumerator RefreshHPAnimated(int character);
    public IEnumerator RefreshXPAnimated();

    public IEnumerator PlayMoveAnimation(int attacker, IMove move);
    public IEnumerator PlayBlinkAnimation(int blinkingPokemon);
    public IEnumerator PlayFaintAnimation(int faintedOwner);
    public IEnumerator PlayInflictStatusAnimation(int owner);
    public IEnumerator PlayStatUpAnimation(int owner);
    public IEnumerator PlayStatDownAnimation(int owner);

    public IEnumerator PlayThrowAnimation();
    public IEnumerator PlayShakeAnimation();
    public void HideOpponent();
    public void ShowOpponent();
    public void HidePokeBallAnimation();

    public IEnumerator MakeOpponentAppear();
    public IEnumerator MakeOpponentDisappear();
    public void OpenBattleMenu(System.Action<BattleOption, bool> callback);
    public void OpenPokemonSwitchSelection(System.Action<ISelectableUIElement, bool> callback, bool forceSelection = false);
    public void OpenMoveSelection(System.Action<ISelectableUIElement, bool> callback, IPokemon pokemon);
    public void OpenBagSelection(System.Action<ISelectableUIElement, bool> callback);
    public void CloseBattleMenu();
    public void ClosePokemonSwitchSelection();
    public void CloseMoveSelection();
    public void CloseBagSelection();
}