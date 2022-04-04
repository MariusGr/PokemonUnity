using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour, IBattleManager
{
    private Character player;
    private Character opponent;
    private Pokemon playerPokemon;
    private Pokemon opponentPokemon;

    void Awake()
    {
        Services.Register(this);
    }

    public void StartNewBattle(Character player, Character opponent)
    {
        this.player = player;
        this.opponent = opponent;

        playerPokemon = player.pokemons[0];
        //Services.Get<IBattleUI>().Start
    }


}
