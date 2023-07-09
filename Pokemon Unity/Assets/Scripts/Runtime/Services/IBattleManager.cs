using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleManager : IService
{
    // TODO: return type void instead of bool
    public void StartNewEncounter(CharacterData playerData, Pokemon wildPokemon, System.Func<bool, bool> encounterEndtionCallback);
    public void StartNewBattle(CharacterData playerData, NPCData opponentData, System.Func<bool, bool> npcBattleEndReactionCallback);
    public bool OpponentIsWild();
}
