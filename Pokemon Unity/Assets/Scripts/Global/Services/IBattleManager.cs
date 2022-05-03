using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleManager : IService
{
    public void StartNewBattle(CharacterData playerData, NPCData opponentData);
    public void DoMove(Move move);
}
