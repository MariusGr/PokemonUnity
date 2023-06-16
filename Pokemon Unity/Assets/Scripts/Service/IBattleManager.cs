using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleManager : IService
{
    // TODO: return type void instead of bool
    public void StartNewEncounter(ICharacterData playerData, IPokemon wildPokemon, System.Func<bool, bool> encounterEndtionCallback);
    public void StartNewBattle(ICharacterData playerData, INPCData opponentData, System.Func<bool, bool> npcBattleEndReactionCallback);
    public bool OpponentIsWild();
}
