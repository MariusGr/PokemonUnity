using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleUI : IService
{
    public void Initialize(CharacterData playerData, Pokemon playerPokemon, Pokemon opponentPokemon);
    public void Initialize(CharacterData playerData, NPCData opponentData, Pokemon playerPokemon, Pokemon opponentPokemon);
    public void Close();
    public void RefreshHP(int character);
    public System.Func<bool> RefreshHPAnimated(int character);
    public System.Func<bool> PlayMoveAnimation(int attacker, Move move);
    public System.Func<bool> PlayBlinkAnimation(int blinkingPokemon);
    public System.Func<bool> PlayFaintAnimation(int faintedOwner);
    public System.Func<bool> MakeOpponentAppear();
    public System.Func<bool> MakeOpponentDisappear();
    public void SetBattleMenuActive(bool active);
    public void SetMoveSelectionActive(bool active);
    public void RefreshMove(Move move);
    public void SetPokemonSwitchSelectionActive(bool active, bool forceSelection = false);
}
