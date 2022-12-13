using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : ManagerWithPokemonManager, IBattleManager
{
    private enum BattleState
    {
        None,
        ChoosingMove,
        BattleMenu,
        Ran,
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
    private int runTryCount = 0;
    private Dictionary<Pokemon, HashSet<Pokemon>> unfaintedPlayerPokemons;
    private HashSet<Pokemon> evolvingPokemons;
    private int playerRunSuccessChance => (playerPokemon.speedUnmodified * 128 / Math.Min(1, opponentPokemon.speedUnmodified) + 30 * runTryCount) % 256;

    private BattleOption playerBattleOption = BattleOption.None;
    private BattleOption opponentBattleOption = BattleOption.None;

    private CharacterData[] characterData;

    private Pokemon wildPokemon;
    private Pokemon playerPokemon => GeActivePokemon(Constants.PlayerIndex);
    private Pokemon opponentPokemon => GeActivePokemon(Constants.OpponentIndex);

    private IBattleUI ui;
    private IEvolutionManager evolutionManager;

    public BattleManager() => Services.Register(this as IBattleManager);

    void Awake()
    {
        ui = Services.Get<IBattleUI>();
        evolutionManager = Services.Get<IEvolutionManager>();
        Initialize();
    }

    private Pokemon GeActivePokemon(int character)
        => character == Constants.OpponentIndex && opponentIsWild ? wildPokemon : characterData[character].pokemons[pokemonIndex[character]];

    private void Reset()
    {
        state = BattleState.None;
        runTryCount = 0;
        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = playerData.GetFirstAlivePokemonIndex();
        isDefeated = new bool[] { false, false };

        ui.Open(playerData, playerPokemon, opponentPokemon);
        unfaintedPlayerPokemons = new Dictionary<Pokemon, HashSet<Pokemon>>();
        evolvingPokemons = new HashSet<Pokemon>();
    }

    public void StartNewEncounter(CharacterData playerData, Pokemon wildPokemon, Func<bool, bool> encounterEndtionCallback)
    {
        print("StartNewEncounter");
        this.wildPokemon = wildPokemon;
        opponentIsWild = true;
        this.playerData = playerData;
        opponentData = null;
        characterData = new CharacterData[] { this.playerData, null };

        Reset();
        unfaintedPlayerPokemons[wildPokemon] = new HashSet<Pokemon>() { playerPokemon };
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
        pokemonIndex[Constants.OpponentIndex] = opponentData.GetFirstAlivePokemonIndex();

        Reset();

        foreach(Pokemon p in this.opponentData.pokemons)
            unfaintedPlayerPokemons[p] = new HashSet<Pokemon>();
        unfaintedPlayerPokemons[opponentPokemon].Add(playerPokemon);

        StartCoroutine(RoundCoroutine(npcBattleEndReactionCallback));
    }

    private bool CharacterHasBeenDefeated(int index, CharacterData characterData)
        => index == Constants.OpponentIndex && (
                !(characterData is null) && characterData.IsDefeated() || characterData is null
        ) || index == Constants.PlayerIndex && characterData.IsDefeated();

    private bool BattleHasEnded() => state.Equals(BattleState.OpponentDefeated) || state.Equals(BattleState.PlayerDefeated) || state.Equals(BattleState.Ran);

    private IEnumerator EndBattle(Func<bool, bool> npcBattleEndReactionCallback)
    {
        ui.Close();

        // Evolve pokemon if any leveled up enough
        foreach (Pokemon p in evolvingPokemons)
            yield return evolutionManager.EvolutionCoroutine(p);

        npcBattleEndReactionCallback?.Invoke(state.Equals(BattleState.OpponentDefeated));
    }

    private IEnumerator RoundCoroutine(Func<bool, bool> npcBattleEndReactionCallback)
    {
        while (true)
        {
            // Get Decisions
            opponentBattleOption = BattleOption.None;
            Move opponentMove;
            int opponentPokemonIndex = -1;
            GetDecisionOpponent(out opponentBattleOption, out opponentMove);
            playerBattleOption = BattleOption.None;
            Move playerMove = null;
            Item playerItem = null;
            int playerPokemonIndex = -1;
            yield return GetDecisionPlayer((battleOption, move, item, index) =>
            {
                playerBattleOption = battleOption;
                playerMove = move;
                playerItem = item;
                playerPokemonIndex = index;
            });

            // Both trainers will fight -> priority decided by attack speed
            if (playerBattleOption == BattleOption.Fight && opponentBattleOption == BattleOption.Fight)
            {
                IEnumerator playerCoroutine = MoveCoroutine(Constants.PlayerIndex, Constants.OpponentIndex, playerMove);
                IEnumerator opponentCoroutine = MoveCoroutine(Constants.OpponentIndex, Constants.PlayerIndex, opponentMove);

                if (playerMove.IsFaster(opponentMove))
                {
                    yield return playerCoroutine;
                    if (BattleHasEnded())
                        break;
                    if (opponentBattleOption == BattleOption.Fight)
                        yield return opponentCoroutine;
                }
                else
                {
                    yield return opponentCoroutine;
                    if (BattleHasEnded())
                        break;
                    if (playerBattleOption == BattleOption.Fight)
                        yield return playerCoroutine;
                }
            }
            else
            {
                // Perform player actions first
                if (playerBattleOption == BattleOption.Fight)
                    yield return MoveCoroutine(Constants.PlayerIndex, Constants.OpponentIndex, playerMove);
                else if (playerBattleOption == BattleOption.PokemonSwitch)
                    yield return ChoosePokemon(Constants.PlayerIndex, playerPokemonIndex);
                else if (playerBattleOption == BattleOption.Bag)
                    yield return PlayerUseItem(playerItem);
                else if (playerBattleOption == BattleOption.Run)
                    break;

                // Secondly, perform opponent actions
                if (opponentBattleOption == BattleOption.Fight)
                    yield return MoveCoroutine(Constants.OpponentIndex, Constants.PlayerIndex, opponentMove);
                else if (opponentBattleOption == BattleOption.PokemonSwitch)
                    yield return ChoosePokemon(Constants.OpponentIndex, opponentPokemonIndex);
                //else if (playerBattleOption == BattleOption.Bag)
                    //TODO
            }

            if (BattleHasEnded())
                break;
        }

        yield return EndBattle(npcBattleEndReactionCallback);
    }

    private IEnumerator PlayerUseItem(Item item)
    {
        if (!item.data.usableOnBattleOpponent)
        {
            // TODO: This is a workaround. It actually only needs to be called in case item has been used on player's active pokemon
            ui.RefreshHP(Constants.PlayerIndex);
            // TODO: do not break if item can be used on own pokemon and usage shall be animated
            yield break;
        }

        yield return dialogBox.DrawText($"{playerData.name} setzt {item.data.fullName} ein!", DialogBoxContinueMode.External);
        if (item.data.catchesPokemon)
        {
            // TODO
        }
    }

    private IEnumerator RunPlayer()
    {
        state = BattleState.Ran;
        return TryRunPlayer(null, true);
    }
    private IEnumerator TryRunPlayer(Action<bool> callback, bool forceSuccess = false)
    {
        ui.CloseBattleMenu();
        runTryCount++;
        bool success = forceSuccess || opponentIsWild && UnityEngine.Random.Range(0, 255) <= playerRunSuccessChance;

        if (success)
            yield return dialogBox.DrawText($"Du bist entkommen!", DialogBoxContinueMode.User);
        else
            yield return dialogBox.DrawText($"Du kannst nicht fliehen!", DialogBoxContinueMode.User);

        dialogBox.Close();
        if (!(callback is null))
            callback(success);
    }

    private IEnumerator GetBattleMenuOption(Action<BattleOption> callback)
    {
        BattleOption choosenOption = BattleOption.None;
        ui.OpenBattleMenu((BattleOption selection, bool goBack) => choosenOption = selection);
        print("wait for play to choose option");
        yield return new WaitUntil(() => choosenOption != BattleOption.None);
        ui.CloseBattleMenu();
        callback(choosenOption);
    }

    private void GetDecisionOpponent(out BattleOption battleOption, out Move opponentMove)
    {
        // TODO: implement more intelligent decision making
        battleOption = BattleOption.Fight;
        opponentMove = GetMoveOpponent();
    }

    private IEnumerator GetDecisionPlayer(Action<BattleOption, Move, Item, int> callback)
    {
        bool goBack = false;
        BattleOption battleOption;
        Move chosenMove = null;
        Item chosenItem = null;
        int choosenPokemon = -1;

        // Battle can only continue if the user chose a move or runs from battle
        while (true)
        {
            // Wait for player to choose from battle menu
            battleOption = BattleOption.None;
            yield return GetBattleMenuOption(choosenOption => battleOption = choosenOption);

            if (battleOption == BattleOption.Run)
            {
                bool successfullyRan = false;
                yield return TryRunPlayer((success) => successfullyRan = success);

                if (successfullyRan)
                    state = BattleState.Ran;
                else
                    battleOption = BattleOption.None;

                break;
            }
            else if (battleOption == BattleOption.Bag)
            {
                goBack = false;
                yield return GetItemPlayer(
                    (item, back) =>
                    {
                        chosenItem = item;
                        goBack = back;
                    });
                if (!goBack) break;
            }
            else if (battleOption == BattleOption.PokemonSwitch)
            {
                goBack = false;
                yield return GetNextPokemonPlayer(
                    (index, back) =>
                    {
                        choosenPokemon = index;
                        goBack = back;
                    });
                if (!goBack) break;
            }
            else
            {
                // Fight!
                // Get player move and perform moves from both sides
                chosenMove = null;
                goBack = false;
                while (chosenMove is null && !goBack)
                    yield return GetMovePlayer((choosenMove, back) =>
                    {
                        chosenMove = choosenMove;
                        goBack = back;
                    });

                // Only continue with battle if player did not want to go back
                if (!goBack) break;
            }
        }

        callback(battleOption, chosenMove, chosenItem, choosenPokemon);
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

        // Let player Choose move
        Move choosenMove = null;
        bool goBack = false;
        ui.OpenMoveSelection((ISelectableUIElement selection, bool back) =>
        {
            goBack = back;
            choosenMove = selection is null ? null : playerPokemon.moves[selection.GetIndex()];
        }, playerPokemon);
        print("wait for play to choose move");
        yield return new WaitUntil(() => choosenMove != null || goBack);
        ui.CloseMoveSelection();

        if (!(choosenMove is null) && choosenMove.pp < 1)
        {
            choosenMove = null;
            yield return dialogBox.DrawText("Keine AP übrig!", DialogBoxContinueMode.User, true);
        }

        callback(choosenMove, goBack);
    }

    private IEnumerator ChooseNextPokemon(int characterIndex)
    {
        if (characterIndex == Constants.OpponentIndex)
            yield return GetNextPokemonOpponent();
        else
        {
            yield return GetNextPokemonPlayer();
        }
    }

    private IEnumerator GetNextPokemonPlayer()
    {
        while (true)
        {
            int chosenOption = 0;
            if (opponentIsWild)
            {
                print("Ask player for after faint reaction");
                yield return dialogBox.DrawChoiceBox("Nächstes Pokemon einwechseln oder besser die Kurve kratzen?", new string[] { "Wechsel", "Flucht" });
                print("Choosen Index  " + dialogBox.GetChosenIndex());
                chosenOption = dialogBox.GetChosenIndex();
            }

            if (chosenOption == 1)
            {
                print("Player Choose Run After Faint");
                yield return RunPlayer();
                yield break;
            }
            else
            {
                int choosenPokemon = -1;
                print("Player Choose Switch After Faint");
                bool goBack = false;
                yield return GetNextPokemonPlayer(
                    (index, back) =>
                    {
                        goBack = back;
                        choosenPokemon = index;
                    }, !opponentIsWild);
                if (!goBack)
                {
                    yield return ChoosePokemon(Constants.PlayerIndex, choosenPokemon);
                    yield break;
                }
            }
        }
    }

    private IEnumerator GetNextPokemonOpponent()
    {
        opponentBattleOption = BattleOption.None;
        for (int i = 0; i < opponentData.pokemons.Length; i++)
        {
            Pokemon pokemon = opponentData.pokemons[i];
            if (!pokemon.isFainted)
            {
                yield return dialogBox.DrawText($"{opponentData.name} wird als nächstes {pokemon.SpeciesName} einsetzen.", DialogBoxContinueMode.User);
                while(true)
                {
                    yield return dialogBox.DrawChoiceBox("Möchtest du dein Pokémon wechseln?");
                    if (dialogBox.GetChosenIndex() == 0)
                    {
                        // Player wants to switch pokemon
                        bool goBack = false;
                        int playerPokemon = -1;
                        yield return GetNextPokemonPlayer(
                            (index, back) =>
                            {
                                playerPokemon = index;
                                goBack = back;
                            });
                        if (!goBack)
                        {
                            yield return ChoosePokemon(Constants.PlayerIndex, playerPokemon);
                            break;
                        }
                    }
                    else
                    {
                        // Player wants to continue with current pokemon
                        dialogBox.Close();
                        break;
                    }
                }
                yield return ChoosePokemon(Constants.OpponentIndex, i);
                break;
            }
        }
    }

    private IEnumerator GetNextPokemonPlayer(Action<int, bool> callback, bool forceSelection = false)
    {
        playerBattleOption = BattleOption.None;
        bool goBack = false;
        int choosenPokemonIndex = -1;
        ui.OpenPokemonSwitchSelection((ISelectableUIElement selection, bool back) =>
        {
            choosenPokemonIndex = selection is null ? -1 : selection.GetIndex();
            goBack = back;
        }, forceSelection);

        print("wait for player to choose pkmn");
        while(true)
        {
            dialogBox.DrawText("Welches Pokemon wirst du als nächstes einsetzen?", DialogBoxContinueMode.External);

            yield return new WaitUntil(() => choosenPokemonIndex > -1 || goBack);

            if (goBack)
                break;

            if (playerData.pokemons[choosenPokemonIndex].isFainted)
            {
                choosenPokemonIndex = -1;
                yield return dialogBox.DrawText($"{playerPokemon.Name}\nist K.O.!", DialogBoxContinueMode.User, true);
                continue;
            }
            else if (choosenPokemonIndex == pokemonIndex[Constants.PlayerIndex])
            {
                choosenPokemonIndex = -1;
                yield return dialogBox.DrawText($"{playerPokemon.Name}\nkämpft bereits!", DialogBoxContinueMode.User, true);
            }
            else
                break;
        }

        dialogBox.Continue();
        dialogBox.Close();
        ui.ClosePokemonSwitchSelection();

        callback(choosenPokemonIndex, goBack);
    }

    private IEnumerator GetItemPlayer(Action<Item, bool> callback)
    {
        playerBattleOption = BattleOption.None;
        bool goBack = false;
        Item chosenItem = null;
        ui.OpenBagSelection((ISelectableUIElement selection, bool back) =>
        {
            chosenItem = selection is null ? null : (Item)selection.GetPayload();
            goBack = back;
        });

        print("wait for player to choose item");
        yield return new WaitUntil(() => !(chosenItem is null) || goBack);
        ui.CloseBagSelection();

        callback(chosenItem, goBack);
    }

    private IEnumerator ChoosePokemon(int characterIndex, int pokemonIndex)
    {
        // TODO Animate pokemon retreat
        this.pokemonIndex[characterIndex] = pokemonIndex;

        unfaintedPlayerPokemons[opponentPokemon].Add(playerPokemon);

        // TODO Animate pokemon deployment
        ui.SwitchToPokemon(characterIndex, characterData[characterIndex].pokemons[pokemonIndex]);
        yield return null;
    }

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

        bool critical = false;
        bool targetFainted = false;
        int damage = 0;
        Effectiveness effectiveness = Effectiveness.Normal;

        if (move.TryHit(attackerPokemon, targetPokemon))
        {
            // Attack hit successfully

            // Animation
            yield return ui.PlayMoveAnimation(attacker, move);
            yield return new WaitForSeconds(1f);
            yield return ui.PlayBlinkAnimation(target);

            // Deal damage
            damage = move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);
            targetFainted = targetPokemon.InflictDamage(damage);
            yield return ui.RefreshHPAnimated(target);
            if (effectiveness != Effectiveness.Normal)
                yield return dialogBox.DrawText(effectiveness, DialogBoxContinueMode.User);
            if (critical)
                yield return dialogBox.DrawText($"Ein Volltreffer!", DialogBoxContinueMode.User);

            // TODO: special branch for attacks with no damage infliction
        }
        else
            // Attack failed
            yield return dialogBox.DrawText($"Attacke von {attackingPokemonIdentifier} geht daneben!", DialogBoxContinueMode.User);

        // Aftermath: Faint, Poison, etc.
        if (targetFainted)
        {
            // target pokemon fainted
            yield return Faint(target, targetPokemonIdentifier);
        }
        else
        {
            // Aftermath e.g. Status effects etc.
            bool attackerFainted = false;
            if (move.data.recoil > 0 && damage > 0)
            {
                yield return dialogBox.DrawText($"{attackingPokemonIdentifier} wird durch Rückstoß getroffen!", DialogBoxContinueMode.Automatic);
                yield return new WaitForSeconds(1f);
                yield return ui.PlayBlinkAnimation(attacker);
                int recoilDamage = (int)(damage * move.data.recoil);
                attackerFainted = attackerPokemon.InflictDamage(recoilDamage);
                yield return ui.RefreshHPAnimated(attacker);
            }

            // TODO wann aftermath für Opponent anwenden, wann Opponent faint
            if (attackerFainted)
            {
                yield return Faint(attacker, attackingPokemonIdentifier);
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

    private IEnumerator Faint(int characterIndex, string pokemonIdentifier)
    {
        yield return dialogBox.DrawText($"{pokemonIdentifier} wurde besiegt!", DialogBoxContinueMode.User);
        yield return ui.PlayFaintAnimation(characterIndex);

        if (characterIndex == Constants.PlayerIndex)
            foreach (HashSet<Pokemon> set in unfaintedPlayerPokemons.Values)
                set.Remove(playerPokemon);
        else
            yield return DistributePlayerXP();

        bool characterHasBeenDefeated = CharacterHasBeenDefeated(characterIndex, characterData[characterIndex]);
        isDefeated[characterIndex] = characterHasBeenDefeated;
        if (!characterHasBeenDefeated)
            yield return ChooseNextPokemon(characterIndex);
    }

    private IEnumerator DistributePlayerXP()
    {
        HashSet<Pokemon> gainingPokemons = unfaintedPlayerPokemons[opponentPokemon];
        int gainedXP = opponentPokemon.GetXPGainedFromFaint(opponentIsWild) / gainingPokemons.Count;
        foreach (Pokemon p in gainingPokemons.Where(p => !p.IsLeveledToMax()))
            yield return GainXP(p, gainedXP);
        dialogBox.Close();
    }

    private IEnumerator GainXP(Pokemon pokemon, int xp)
    {
        yield return dialogBox.DrawText($"{pokemon.Name} erhält {xp} EP!", DialogBoxContinueMode.User);
        pokemon.GainXP(xp);
        while(true)
        {
            yield return ui.RefreshXPAnimated();
            if (!pokemon.WillGrowLevel())
                break;
            yield return pokemonManager.GrowLevel(pokemon, ui.RefreshPlayerStats);
            if (pokemon.WillEvolve())
                evolvingPokemons.Add(pokemon);
            // TODO: Show stats
            ui.ResetXP();
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
