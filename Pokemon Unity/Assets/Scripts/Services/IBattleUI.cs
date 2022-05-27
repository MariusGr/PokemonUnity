using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleUI : IService
{
    public void Initialize(CharacterData playerData, NPCData opponentData, Pokemon playerPokemon, Pokemon opponentPokemon);
    public void RefreshHP(int character);
    public System.Func<bool> RefreshHPAnimated(int character);
    public System.Func<bool> PlayMoveAnimation(int attacker, Move move);
    public void SetMoveSelectionActive(bool active);
}
