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
    private bool battleEnded = false;
    private CharacterData playerData;
    private NPCData opponentData;
    private int[] pokemonIndex = new int[] { 0, 0 };

    public delegate void UserMoveChooseEventHandler(Move move);
    public event UserMoveChooseEventHandler UserChooseMoveEvent;

    private CharacterData[] characterData;

    private Pokemon playerPokemon => GeActivePokemon(Constants.PlayerIndex);
    private Pokemon opponentPokemon => GeActivePokemon(Constants.OpponentIndex);

    private IBattleUI ui;
    private IDialogBox dialogBox;

    public BattleManager() => Services.Register(this as IBattleManager);

    void Awake()
    {
        ui = Services.Get<IBattleUI>();
        ui.Close();
        dialogBox = Services.Get<IDialogBox>();
    }

    private Pokemon GeActivePokemon(int character) => characterData[character].pokemons[pokemonIndex[character]];

    public void StartNewBattle(CharacterData playerData, NPCData opponentData)
    {
        print("StartNewBattle");
        battleEnded = false;
        this.playerData = playerData;
        this.opponentData = opponentData;
        characterData = new CharacterData[] { this.playerData, this.opponentData };

        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = 0;
        pokemonIndex[Constants.OpponentIndex] = 0;

        EventManager.Pause();
        ui.Initialize(this.playerData, this.opponentData, playerPokemon, opponentPokemon);

        StartCoroutine(RoundCoroutine());
    }

    private void EndBattle()
    {
        battleEnded = true;
        ui.Close();
        EventManager.Unpause();
    }

    private IEnumerator RoundCoroutine()
    {
        while (true)
        {
            Move opponentMove = GetOpponentMove();
            Move playerMove = null;
            yield return GetPlayerMove(choosenMove => playerMove = choosenMove);

            IEnumerator playerCoroutine = MoveCoroutine(Constants.PlayerIndex, Constants.OpponentIndex, playerMove);
            IEnumerator opponentCoroutine = MoveCoroutine(Constants.OpponentIndex, Constants.PlayerIndex, opponentMove);

            if (playerMove.IsFaster(opponentMove))
            {
                yield return playerCoroutine;
                if (battleEnded)
                    break;
                yield return opponentCoroutine;

            }
            else
            {
                yield return opponentCoroutine;
                if (battleEnded)
                    break;
                yield return playerCoroutine;
            }

            if (battleEnded)
                break;
        }
    }

    private void GetNextPokemon(int character)
    {
        if (character == Constants.OpponentIndex)
        {

        }
    }

    private Move GetOpponentMove()
    {
        // TODO: implement more intelligent move choose
        return opponentPokemon.moves[UnityEngine.Random.Range(0, opponentPokemon.moves.Count)];
    }

    private IEnumerator GetPlayerMove(Action<Move> callback)
    {
        ui.SetMoveSelectionActive(true);

        Move choosenMove = null;
        UserMoveChooseEventHandler action = (Move move) => choosenMove = move;
        UserChooseMoveEvent += action;
        yield return new WaitUntil(() => choosenMove != null);
        ui.SetMoveSelectionActive(false);
        UserChooseMoveEvent -= action;
        callback(choosenMove);
    }

    public void ChoosePlayerMove(Move move) => UserChooseMoveEvent?.Invoke(move);

    string GetUniqueIdentifier(Pokemon pokemon, Pokemon other, CharacterData character)
        => pokemon.Name == other.Name ? $"{character.nameGenitive} {pokemon.Name}" : pokemon.Name;

    private IEnumerator MoveCoroutine(int attacker, int target, Move move)
    {
        CharacterData attackerCharacter = characterData[attacker];
        CharacterData targetCharacter = characterData[target];
        Pokemon attackerPokemon = GeActivePokemon(attacker);
        Pokemon targetPokemon = GeActivePokemon(target);

        // Play attack animation
        string attackingPokemonIdentifier = GetUniqueIdentifier(attackerPokemon, targetPokemon, attackerCharacter);
        string targetPokemonIdentifier = GetUniqueIdentifier(targetPokemon, attackerPokemon, targetCharacter);
        yield return dialogBox.DrawText($"{attackingPokemonIdentifier} setzt {move.data.fullName} ein!", DialogBoxCloseMode.External);
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(ui.PlayMoveAnimation(attacker, move));
        yield return new WaitForSeconds(1f);
        
        // Deal Damage to target
        // TODO: special branch for attacks with no damage infliction
        bool critical;
        Effectiveness effectiveness;
        int damage = move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);

        bool fainted = targetPokemon.InflictDamage(damage);

        yield return new WaitWhile(ui.RefreshHPAnimated(target));

        if (effectiveness != Effectiveness.Normal)
            yield return dialogBox.DrawText(effectiveness, DialogBoxCloseMode.User);

        if (critical)
            yield return dialogBox.DrawText($"Ein Volltreffer!", DialogBoxCloseMode.User);

        // Aftermath: Faint, Poison, etc.
        if (fainted)
        {
            // target pokemon fainted
            yield return dialogBox.DrawText($"{targetPokemonIdentifier} wurde besiegt!", DialogBoxCloseMode.User);
            yield return new WaitWhile(ui.PlayFaintAnimation(target));
            //yield return new WaitForSeconds(1f);

            if (targetCharacter.IsDefeated())
            {
                // target character has lost the battle
                ui.MakeOpponentAppear();
                yield return dialogBox.DrawText($"{targetCharacter.name} wurde besiegt!", DialogBoxCloseMode.User);
                if (target == Constants.OpponentIndex)
                {
                    // AI opponent has been defeated
                    yield return dialogBox.DrawText(new string[]
                    {
                        ((NPCData)targetCharacter).battleDefeatText,
                        $"Du erhältst {targetCharacter.GetPriceMoneyFormatted()}.",
                    }, DialogBoxCloseMode.User);
                }

                EndBattle();
            }
            else
            {
                // target character must choose new pokemon
            }
        }
        else
        {
            // Status effects etc.
        }

        dialogBox.Close();
    }
}
