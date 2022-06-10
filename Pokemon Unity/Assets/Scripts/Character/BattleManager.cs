using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour, IBattleManager
{
    private enum BattleState
    {
        None,
        ChoosingMove,
        BatlleMenu,
    }

    private BattleState state;
    private CharacterData playerData;
    private NPCData opponentData;
    private int activePlayer = 0;
    private int playerPokemonIndex = 0;
    private int opponentPokemonIndex = 0;

    private CharacterData[] characterData;

    private Pokemon playerPokemon => GeActivePokemon(playerData);
    private Pokemon opponentPokemon => GeActivePokemon(opponentData);

    private IBattleUI ui;
    private IDialogBox dialogBox;

    public BattleManager() => Services.Register(this as IBattleManager);

    void Awake()
    {
        ui = Services.Get<IBattleUI>();
        dialogBox = Services.Get<IDialogBox>();
    }

    private Pokemon GeActivePokemon(CharacterData character) => character.pokemons[playerPokemonIndex];

    public void StartNewBattle(CharacterData playerData, NPCData opponentData)
    {
        this.playerData = playerData;
        this.opponentData = opponentData;
        characterData = new CharacterData[] { this.playerData, this.opponentData };

        state = BattleState.ChoosingMove;
        playerPokemonIndex = 0;
        opponentPokemonIndex = 0;
        activePlayer = 0;

        ui.Initialize(this.playerData, this.opponentData, playerPokemon, opponentPokemon);

        StartRound();
    }

    private void StartRound()
    {
        activePlayer = 0;
    }

    public void DoPlayerMove(Move move)
    {
        ui.SetMoveSelectionActive(false);
        StartCoroutine(MoveRoutine(Constants.PlayerIndex, Constants.OpponentIndex, move));
    }

    private IEnumerator MoveRoutine(int attacker, int target, Move move)
    {
        CharacterData attackerCharacter = characterData[attacker];
        CharacterData targetCharacter = characterData[target];

        Pokemon attackerPokemon = GeActivePokemon(attackerCharacter);
        Pokemon targetPokemon = GeActivePokemon(targetCharacter);
        
        // Play attack animation
        print(Services.Get<IDialogBox>());
        yield return dialogBox.DrawText(new string[] { $"{attackerPokemon.Name} setzt {move.data.fullName} ein!" }, DialogBoxCloseMode.External);
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(ui.PlayMoveAnimation(attacker, move));
        yield return new WaitForSeconds(1f);
        
        // Deal Damage to target
        bool critical;
        Effectiveness effectiveness;
        int damage = move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);

        targetPokemon.InflictDamage(damage);

        yield return new WaitWhile(ui.RefreshHPAnimated(target));

        if (effectiveness != Effectiveness.Normal)
            yield return dialogBox.DrawText(effectiveness, DialogBoxCloseMode.User);

        if (critical)
            yield return dialogBox.DrawText(new string[] { $"Es war ein kritischer Treffer!" }, DialogBoxCloseMode.User);


        dialogBox.Close();
    }
}
