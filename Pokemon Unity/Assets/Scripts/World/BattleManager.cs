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
    private bool[] isDefeated = new bool[] { false, false };
    private bool opponentIsWild = false;

    public delegate void UserBattleOptionChooseEventHandler(BattleOption option);
    public delegate void UserMoveChooseEventHandler(Move move, bool goBack);
    public delegate void UserPokemonChooseEventHandler(int index);
    public event UserBattleOptionChooseEventHandler UserChooseBattleOptionEvent;
    public event UserMoveChooseEventHandler UserChooseMoveEvent;
    public event UserPokemonChooseEventHandler UserChoosePokemonEvent;

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
        characterData = new CharacterData[] { this.playerData, null };

        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = 0;
        isDefeated = new bool[] { false, false };

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
        isDefeated = new bool[] { false, false };

        ui.Initialize(this.playerData, this.opponentData, playerPokemon, opponentPokemon);

        StartCoroutine(RoundCoroutine(npcBattleEndReactionCallback));
    }

    private bool CharacterHasBeenDefeated(int index, CharacterData characterData)
        => index == Constants.OpponentIndex && (
                !(characterData is null) && characterData.IsDefeated() || characterData is null
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
            // Get opponent move for this round
            Move opponentMove = GetMoveOpponent();
            Move playerMove = null;
            bool goBack = false;

            // Battle can only continue if the user chose a move or runs from battle
            while (true)
            {
                // Wait for player to choose from battle menu
                BattleOption battleOption = BattleOption.None;
                yield return GetBattleMenuOption(choosenOption => battleOption = choosenOption);

                if (battleOption == BattleOption.Run)
                {
                    yield return TryRun();
                    if (BattleHasEnded())
                        break;
                }
                else if (battleOption == BattleOption.Bag)
                {
                    //TODO
                }
                else if (battleOption == BattleOption.PokemonSwitch)
                {
                    yield return GetNextPokemonPlayer();
                }
                else
                {
                    // Fight!
                    // Get player move and perform moves from both sides
                    playerMove = null;
                    goBack = false;
                    while(playerMove is null && !goBack)
                        yield return GetMovePlayer((choosenMove, back) =>
                        {
                            playerMove = choosenMove;
                            goBack = back;
                        });

                    // Only continue with battle if player did not want to go back
                    if (!goBack) break;
                }
            }

            // In case running was succesfull, battle needs to end immideatly
            if (BattleHasEnded())
                break;

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

    private IEnumerator TryRun()
    {
        ui.SetBattleMenuActive(false);
        yield return dialogBox.DrawText($"Du kannst nicht fliehen!", DialogBoxContinueMode.User);
        dialogBox.Close();
        ui.SetBattleMenuActive(true);
        //TODO: aus encounter fliehen - wovon hängt Erfolg ab?
    }

    private IEnumerator GetBattleMenuOption(Action<BattleOption> callback)
    {
        ui.SetBattleMenuActive(true);
        BattleOption choosenOption = BattleOption.None;
        UserBattleOptionChooseEventHandler action = (BattleOption option) => choosenOption = option;
        UserChooseBattleOptionEvent += action;
        print("wait for play to choose option");
        yield return new WaitUntil(() => choosenOption != BattleOption.None);
        ui.SetBattleMenuActive(false);
        UserChooseBattleOptionEvent -= action;
        callback(choosenOption);
    }

    private void GetNextPokemon(int character)
    {
        if (character == Constants.OpponentIndex)
        {

        }
    }

    private Move GetMoveOpponent()
    {
        // TODO: implement more intelligent move choose
        return opponentPokemon.moves[UnityEngine.Random.Range(0, opponentPokemon.moves.Count)];
    }

    private IEnumerator GetMovePlayer(Action<Move, bool> callback)
    {
        if(!playerPokemon.HasUsableMoves())
        {
            struggleMove.SetPokemon(playerPokemon);
            callback(struggleMove, false);
            yield break;
        }

        ui.SetMoveSelectionActive(true);

        Move choosenMove = null;
        bool goBack = false;
        UserMoveChooseEventHandler action =
            (Move move, bool back) =>
            {
                choosenMove = move;
                goBack = back;
            };
        UserChooseMoveEvent += action;
        print("wait for play to choose move");
        yield return new WaitUntil(() => choosenMove != null || goBack);
        ui.SetMoveSelectionActive(false);
        UserChooseMoveEvent -= action;

        if (choosenMove.pp < 1)
        {
            choosenMove = null;
            yield return dialogBox.DrawText("Keine AP übrig!", DialogBoxContinueMode.User, true);
        }

        callback(choosenMove, goBack);
    }

    private IEnumerator ChooseNextPokemon(int characterIndex)
    {
        if (characterIndex == Constants.OpponentIndex)
            GetNextPokemonOpponent();
        else
            yield return GetNextPokemonPlayer();
    }

    private void GetNextPokemonOpponent()
    {
        for (int i = 0; i < opponentData.pokemons.Length; i++)
        {
            if (!opponentData.pokemons[i].isFainted)
            {
                ChoosePokemon(Constants.OpponentIndex, i);
                return;
            }
        }
    }

    private IEnumerator GetNextPokemonPlayer()
    {
        ui.SetPokemonSwitchSelectionActive(true, true);

        int choosenPokemonIndex = -1;
        UserPokemonChooseEventHandler action = (int index) => choosenPokemonIndex = index;
        UserChoosePokemonEvent += action;
        print("wait for play to choose pkmn");
        yield return new WaitUntil(() => choosenPokemonIndex > -1);
        ui.SetPokemonSwitchSelectionActive(false);
        UserChoosePokemonEvent -= action;
        ChoosePokemon(Constants.PlayerIndex, choosenPokemonIndex);
    }

    private void ChoosePokemon(int characterIndex, int pokemonIndex)
        => this.pokemonIndex[characterIndex] = pokemonIndex;

    public void ChooseBattleMenuOption(BattleOption option) => UserChooseBattleOptionEvent?.Invoke(option);
    public void ChoosePlayerMove(Move move, bool goBack) => UserChooseMoveEvent?.Invoke(move, goBack);
    public void ChoosePlayerPokemon(int index) => UserChoosePokemonEvent?.Invoke(index);

    string GetUniqueIdentifier(Pokemon pokemon, Pokemon other, CharacterData character)
        => pokemon.Name == other.Name ? (
            character is null ? (pokemon == opponentPokemon ? $"Wildes {pokemon.Name}" : pokemon.Name) : $"{character.nameGenitive} {pokemon.Name}"
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
        ui.RefreshMove(move);

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

        bool targetFainted = targetPokemon.InflictDamage(damage);

        yield return new WaitWhile(ui.RefreshHPAnimated(target));

        if (effectiveness != Effectiveness.Normal)
            yield return dialogBox.DrawText(effectiveness, DialogBoxContinueMode.User);

        if (critical)
            yield return dialogBox.DrawText($"Ein Volltreffer!", DialogBoxContinueMode.User);

        // Aftermath: Faint, Poison, etc.
        if (targetFainted)
        {
            // target pokemon fainted
            yield return Faint(target, targetPokemonIdentifier);
            if (!isDefeated[target])
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

            // TODO wann aftermath für Opponent anwenden, wann Opponent faint
            if (attackerFainted)
            {
                yield return Faint(attacker, attackingPokemonIdentifier);
                if (!isDefeated[attacker])
                {
                    // attacker character must choose new pokemon
                }
            }
        }

        // If one character has been defeated, detect which one (player always looses when out of pokemon)
        if (isDefeated[Constants.PlayerIndex])
            yield return Defeat(Constants.PlayerIndex, opponentData, playerData);
        else if (isDefeated[Constants.OpponentIndex])
            yield return Defeat(Constants.OpponentIndex, playerData, opponentData);
        else
            dialogBox.Close();
    }

    private IEnumerator Faint(int index, string pokemonIdentifier)
    {
        yield return dialogBox.DrawText($"{pokemonIdentifier} wurde besiegt!", DialogBoxContinueMode.User);
        yield return new WaitWhile(ui.PlayFaintAnimation(index));

        bool characterHasBeenDefeated = CharacterHasBeenDefeated(index, characterData[index]);
        isDefeated[index] = characterHasBeenDefeated;
        if (!characterHasBeenDefeated)
        {
            // target character must choose new pokemon
        }
    }

    private IEnumerator Defeat(int loserIndex, CharacterData winner, CharacterData loser)
    {
        bool winnerIsTrainer = !(winner is null);
        bool loserIsTrainer = !(loser is null);

        if (!opponentIsWild)
            ui.MakeOpponentAppear();
        if (loserIsTrainer)
            yield return dialogBox.DrawText($"{loser.name} wurde besiegt!", DialogBoxContinueMode.User);
        if (loserIndex == Constants.OpponentIndex)
        {
            // AI opponent has been defeated
            if (loserIsTrainer)
            {
                NPCData npc = (NPCData)loser;
                yield return dialogBox.DrawText(new string[]
                {
                    npc.battleDefeatText,
                    $"Du erhältst {loser.GetPriceMoneyFormatted()}.",
                }, DialogBoxContinueMode.User);
                ((PlayerData)winner).GiveMoney(loser.GetPriceMoney());
                npc.hasBeenDefeated = true;
            }
            state = BattleState.OpponentDefeated;
        }
        else
        {
            // player has been defeated
            PlayerData playerData = (PlayerData)loser;

            if (winnerIsTrainer)
            {
                NPCData npc = (NPCData)winner;
                yield return dialogBox.DrawText(new string[]
                {
                            npc.battleWinText,
                            $"Dir werden {loser.GetPriceMoneyFormatted()} abgezogen.",
                }, DialogBoxContinueMode.User);
                playerData.TakeMoney(loser.GetPriceMoney());
                winner.HealAllPokemons();
            }

            yield return dialogBox.DrawText($"Tja... ab ins Poke-Center. Scheiße gelaufen...", DialogBoxContinueMode.User);

            loser.HealAllPokemons();
            Character.PlayerCharacter.transform.position = playerData.lastPokeCenterEntrance.position;
            state = BattleState.PlayerDefeated;
        }
    }
}
