using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleManager : IService
{
    // TODO: return type void instead of bool
    public void StartNewBattle(CharacterData playerData, NPCData opponentData, System.Func<bool, bool> npcBattleEndtionCallback);
    public void EndBattle();
    public void ChoosePlayerMove(Move move);
}
