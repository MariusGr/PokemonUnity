using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour, IBattleManager
{
    private CharacterData playerData;
    private NPCData opponentData;
    private Pokemon playerPokemon;
    private Pokemon opponentPokemon;

    void Awake() => Services.Register(this as IBattleManager);

    public void StartNewBattle(CharacterData playerData, NPCData opponentData)
    {
        this.playerData = playerData;
        this.opponentData = opponentData;

        playerPokemon = playerData.pokemons[0];
        Services.Get<IBattleUI>().Initialize(this.playerData, this.opponentData);
    }

    public void DoMove(Move move)
    {

    }
}
