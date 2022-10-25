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

    [SerializeField] Move struggleMove;

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
        EventManager.Pause();
        this.wildPokemon = wildPokemon;
        this.wildPokemon.Initialize();
        opponentIsWild = true;
        state = BattleState.None;
        this.playerData = playerData;
        opponentData = null;
        characterData = new CharacterData[] { this.playerData };

        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = 0;

        ui.Initialize(this.playerData, playerPokemon, opponentPokemon);

        StartCoroutine(RoundCoroutine(encounterEndtionCallback));
    }

    public void StartNewBattle(CharacterData playerData, NPCData opponentData, Func<bool, bool> npcBattleEndReactionCallback)
    {
        print("StartNewBattle");
        EventManager.Pause();
        opponentIsWild = false;
        state = BattleState.None;
        this.playerData = playerData;
        this.opponentData = opponentData;
        characterData = new CharacterData[] { this.playerData, this.opponentData };

        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = 0;
        pokemonIndex[Constants.OpponentIndex] = 0;

        ui.Initialize(this.playerData, this.opponentData, playerPokemon, opponentPokemon);

        StartCoroutine(RoundCoroutine(npcBattleEndReactionCallback));
    }

    private bool CharacterHasBeenDefeated(int index, CharacterData characterData)
        => index == Constants.OpponentIndex && (
                !opponentIsWild && characterData.IsDefeated() || opponentIsWild
        ) || index == Constants.PlayerIndex && characterData.IsDefeated();

    private bool BattleHasEnded() => state.Equals(BattleState.OpponentDefeated) || state.Equals(BattleState.PlayerDefeated);

    private void EndBattle(Func<bool, bool> npcBattleEndReactionCallback)
    {
        ui.Close();
        npcBattleEndReactionCallback?.Invoke(state.Equals(BattleState.OpponentDefeated));
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

        EndBattle(npcBattleEndReactionCallback);
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
        if(!playerPokemon.HasUsableMoves())
        {
            struggleMove.SetPokemon(playerPokemon);
            callback(struggleMove);
            yield break;
        }

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
            opponentIsWild ? (pokemon == opponentPokemon ? $"Wildes {pokemon.Name}" : pokemon.Name) : $"{character.nameGenitive} {pokemon.Name}"
        ) : pokemon.Name;

    private IEnumerator MoveCoroutine(int attacker, int target, Move move)
    {
        CharacterData attackerCharacter = null;
        CharacterData targetCharacter = null;
        if (target == Constants.PlayerIndex || !opponentIsWild)
            targetCharacter = characterData[target];
        if (attacker == Constants.PlayerIndex || !opponentIsWild)
            attackerCharacter = characterData[attacker];
        Pokemon attackerPokemon = GeActivePokemon(attacker);
        Pokemon targetPokemon = GeActivePokemon(target);
        move.DecrementPP();

        // Play attack animation
        string attackingPokemonIdentifier = GetUniqueIdentifier(attackerPokemon, targetPokemon, attackerCharacter);
        string targetPokemonIdentifier = GetUniqueIdentifier(targetPokemon, attackerPokemon, targetCharacter);

        // Display usage text
        string defaultUsageText = $"{attackingPokemonIdentifier} setzt {move.data.fullName} ein!";
        string usageText = null;
        if (move.data.hasSpecialUsageText)
        {
            usageText = move.data.specialUsageText;
            usageText = TextKeyManager.ReplaceKey(TextKeyManager.TextKeyAttackerPokemon, usageText, attackingPokemonIdentifier);
            usageText = TextKeyManager.ReplaceKey(TextKeyManager.TextKeyDefaultUsageText, usageText, defaultUsageText);
        }
        else
            usageText = defaultUsageText;

        yield return dialogBox.DrawText(usageText, DialogBoxContinueMode.Automatic);

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
            yield return Faint(target, targetPokemonIdentifier);

            if (CharacterHasBeenDefeated(target, targetCharacter))
            {
                // target character has lost the battle
                yield return Defeat(target, attackerCharacter, targetCharacter);
            }
            else
            {
                // target character must choose new pokemon
            }
        }
        else
        {
            // Aftermath e.g. Status effects etc.
            //dialogBox.Close();
            bool attackerFainted = false;
            if (move.data.recoil > 0)
            {
                yield return dialogBox.DrawText($"{attackingPokemonIdentifier} wird durch Rückstoß getroffen!", DialogBoxContinueMode.Automatic);
                yield return new WaitForSeconds(1f);
                yield return new WaitWhile(ui.PlayBlinkAnimation(attacker));
                int recoilDamage = (int)(damage * move.data.recoil);
                attackerFainted = attackerPokemon.InflictDamage(recoilDamage);
                yield return new WaitWhile(ui.RefreshHPAnimated(attacker));
            }

            if (attackerFainted)
            {
                yield return Faint(attacker, attackingPokemonIdentifier);
                if (CharacterHasBeenDefeated(attacker, attackerCharacter))
                    if (attacker == Constants.PlayerIndex)
                    {
                        yield return Defeat(attacker, targetCharacter, attackerCharacter);
                    }
                    else
                    {
                        // attacker character must choose new pokemon
                    }
            }
        }
    }

    private IEnumerator Faint(int index, string pokemonIdentifier)
    {
        yield return dialogBox.DrawText($"{pokemonIdentifier} wurde besiegt!", DialogBoxContinueMode.User);
        yield return new WaitWhile(ui.PlayFaintAnimation(index));
    }

    private IEnumerator Defeat(int target, CharacterData attackerCharacter, CharacterData targetCharacter)
    {
        if (!opponentIsWild)
            ui.MakeOpponentAppear();
        if (opponentIsWild && target == Constants.PlayerIndex || !opponentIsWild)
            yield return dialogBox.DrawText($"{targetCharacter.name} wurde besiegt!", DialogBoxContinueMode.User);
        if (target == Constants.OpponentIndex)
        {
            // AI opponent has been defeated
            if (!opponentIsWild)
            {
                NPCData npc = (NPCData)targetCharacter;
                yield return dialogBox.DrawText(new string[]
                {
                            npc.battleDefeatText,
                            $"Du erhältst {targetCharacter.GetPriceMoneyFormatted()}.",
                }, DialogBoxContinueMode.User);
                ((PlayerData)attackerCharacter).GiveMoney(targetCharacter.GetPriceMoney());
                npc.hasBeenDefeated = true;
            }
            state = BattleState.OpponentDefeated;
        }
        else
        {
            // player has been defeated
            PlayerData playerData = (PlayerData)targetCharacter;

            if (!opponentIsWild)
            {
                NPCData npc = (NPCData)attackerCharacter;
                yield return dialogBox.DrawText(new string[]
                {
                            npc.battleWinText,
                            $"Dir werden {targetCharacter.GetPriceMoneyFormatted()} abgezogen.",
                }, DialogBoxContinueMode.User);
                playerData.TakeMoney(targetCharacter.GetPriceMoney());
                attackerCharacter.HealAllPokemons();
            }

            yield return dialogBox.DrawText($"Tja... ab ins Poke-Center. Scheiße gelaufen...", DialogBoxContinueMode.User);

            targetCharacter.HealAllPokemons();
            Character.PlayerCharacter.transform.position = playerData.lastPokeCenterEntrance.position;
            state = BattleState.PlayerDefeated;
        }
    }
}
