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
        Fled,
        OpponentDefeated,
        PlayerDefeated,
    }

    private BattleState state;
    private CharacterData playerData;
    private NPCData opponentData;
    private int[] pokemonIndex = new int[] { 0, 0 };
    private bool opponentIsWild = false;

    public delegate void UserMoveChooseEventHandler(Move move);
    public event UserMoveChooseEventHandler UserChooseMoveEvent;

    private CharacterData[] characterData;

    private Pokemon wildPokemon;
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

    private Pokemon GeActivePokemon(int character)
        => character == Constants.OpponentIndex && opponentIsWild ? wildPokemon : characterData[character].pokemons[pokemonIndex[character]];

    public void StartNewEncounter(CharacterData playerData, Pokemon wildPokemon, System.Func<bool, bool> encounterEndtionCallback)
    {
        print("StartNewEncounter");
        this.wildPokemon = wildPokemon;
        this.wildPokemon.Initialize();
        opponentIsWild = true;
        state = BattleState.None;
        this.playerData = playerData;
        opponentData = null;
        characterData = new CharacterData[] { this.playerData };

        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = 0;

        EventManager.Pause();
        ui.Initialize(this.playerData, playerPokemon, opponentPokemon);

        StartCoroutine(RoundCoroutine(encounterEndtionCallback));
    }

    public void StartNewBattle(CharacterData playerData, NPCData opponentData, Func<bool, bool> npcBattleEndReactionCallback)
    {
        print("StartNewBattle");
        opponentIsWild = false;
        state = BattleState.None;
        this.playerData = playerData;
        this.opponentData = opponentData;
        characterData = new CharacterData[] { this.playerData, this.opponentData };

        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = 0;
        pokemonIndex[Constants.OpponentIndex] = 0;

        EventManager.Pause();
        ui.Initialize(this.playerData, this.opponentData, playerPokemon, opponentPokemon);

        StartCoroutine(RoundCoroutine(npcBattleEndReactionCallback));
    }

    private bool BattleHasEnded() => state.Equals(BattleState.OpponentDefeated) || state.Equals(BattleState.PlayerDefeated);

    public void EndBattle()
    {
        ui.Close();
        EventManager.Unpause();
    }

    private IEnumerator RoundCoroutine(Func<bool, bool> npcBattleEndReactionCallback)
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
                if (BattleHasEnded())
                    break;
                yield return opponentCoroutine;

            }
            else
            {
                yield return opponentCoroutine;
                if (BattleHasEnded())
                    break;
                yield return playerCoroutine;
            }

            if (BattleHasEnded())
                break;
        }

        npcBattleEndReactionCallback?.Invoke(state.Equals(BattleState.OpponentDefeated));
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
        print("wait for play to choose move");
        yield return new WaitUntil(() => choosenMove != null);
        ui.SetMoveSelectionActive(false);
        UserChooseMoveEvent -= action;
        callback(choosenMove);
    }

    public void ChoosePlayerMove(Move move) => UserChooseMoveEvent?.Invoke(move);

    string GetUniqueIdentifier(Pokemon pokemon, Pokemon other, CharacterData character)
        => pokemon.Name == other.Name ? (
            opponentIsWild ? $"Wildes {pokemon.Name}" : $"{character.nameGenitive} {pokemon.Name}"
        ) : pokemon.Name;

    private IEnumerator MoveCoroutine(int attacker, int target, Move move)
    {
        CharacterData attackerCharacter = characterData[attacker];
        CharacterData targetCharacter = null;
        if (!opponentIsWild)
            targetCharacter = characterData[target];
        Pokemon attackerPokemon = GeActivePokemon(attacker);
        Pokemon targetPokemon = GeActivePokemon(target);

        // Play attack animation
        string attackingPokemonIdentifier = GetUniqueIdentifier(attackerPokemon, targetPokemon, attackerCharacter);
        string targetPokemonIdentifier = GetUniqueIdentifier(targetPokemon, attackerPokemon, targetCharacter);
        yield return dialogBox.DrawText($"{attackingPokemonIdentifier} setzt {move.data.fullName} ein!", DialogBoxContinueMode.External);
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(ui.PlayMoveAnimation(attacker, move));
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(ui.PlayBlinkAnimation(target));

        // Deal Damage to target
        // TODO: special branch for attacks with no damage infliction
        bool critical;
        Effectiveness effectiveness;
        int damage = move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);

        bool fainted = targetPokemon.InflictDamage(damage);

        yield return new WaitWhile(ui.RefreshHPAnimated(target));

        if (effectiveness != Effectiveness.Normal)
            yield return dialogBox.DrawText(effectiveness, DialogBoxContinueMode.User);

        if (critical)
            yield return dialogBox.DrawText($"Ein Volltreffer!", DialogBoxContinueMode.User);

        // Aftermath: Faint, Poison, etc.
        if (fainted)
        {
            // target pokemon fainted
            yield return dialogBox.DrawText($"{targetPokemonIdentifier} wurde besiegt!", DialogBoxContinueMode.User);
            yield return new WaitWhile(ui.PlayFaintAnimation(target));
            //yield return new WaitForSeconds(1f);

            if (targetCharacter.IsDefeated())
            {
                // target character has lost the battle
                ui.MakeOpponentAppear();
                yield return dialogBox.DrawText($"{targetCharacter.name} wurde besiegt!", DialogBoxContinueMode.User);
                if (target == Constants.OpponentIndex)
                {
                    // AI opponent has been defeated
                    NPCData npc = (NPCData)targetCharacter;
                    yield return dialogBox.DrawText(new string[]
                    {
                        npc.battleDefeatText,
                        $"Du erhältst {targetCharacter.GetPriceMoneyFormatted()}.",
                    }, DialogBoxContinueMode.User);
                    ((PlayerData)attackerCharacter).GiveMoney(targetCharacter.GetPriceMoney());
                    npc.hasBeenDefeated = true;
                    state = BattleState.OpponentDefeated;
                }
                else
                {
                    // player has been defeated
                    NPCData npc = (NPCData)attackerCharacter;
                    yield return dialogBox.DrawText(new string[]
                    {
                        npc.battleWinText,
                        $"Dir werden {targetCharacter.GetPriceMoneyFormatted()} abgezogen.",
                    }, DialogBoxContinueMode.User);
                    PlayerData playerData = (PlayerData)targetCharacter;
                    playerData.TakeMoney(targetCharacter.GetPriceMoney());
                    attackerCharacter.HealAllPokemons();
                    targetCharacter.HealAllPokemons();
                    Character.PlayerCharacter.transform.position = playerData.lastPokeCenterEntrance.position;
                    state = BattleState.PlayerDefeated;
                }
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
    }
}
