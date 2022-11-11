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
    private int playerRunSuccessChance => (playerPokemon.speedUnmodified * 128 / Math.Min(1, opponentPokemon.speedUnmodified) + 30 * runTryCount) % 256;

    private BattleOption playerBattleOption = BattleOption.None;
    private BattleOption opponentBattleOption = BattleOption.None;

    public delegate void UserBattleOptionChooseEventHandler(BattleOption option);
    public delegate void UserMoveChooseEventHandler(Move move, bool goBack);
    public delegate void UserPokemonChooseEventHandler(int index, bool goBack);
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

    private void Initialize()
    {
        state = BattleState.None;
        runTryCount = 0;
        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = playerData.GetFirstAlivePokemonIndex();
        isDefeated = new bool[] { false, false };

        ui.Open();
        ui.Initialize(this.playerData, playerPokemon, opponentPokemon);
        unfaintedPlayerPokemons = new Dictionary<Pokemon, HashSet<Pokemon>>();
    }

    public void StartNewEncounter(CharacterData playerData, Pokemon wildPokemon, System.Func<bool, bool> encounterEndtionCallback)
    {
        print("StartNewEncounter");
        this.wildPokemon = wildPokemon;
        this.wildPokemon.Initialize();
        opponentIsWild = true;
        this.playerData = playerData;
        opponentData = null;
        characterData = new CharacterData[] { this.playerData, null };

        Initialize();
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

        Initialize();

        foreach(Pokemon p in this.opponentData.pokemons)
            unfaintedPlayerPokemons[opponentPokemon] = new HashSet<Pokemon>();
        unfaintedPlayerPokemons[opponentPokemon].Add(playerPokemon);

        StartCoroutine(RoundCoroutine(npcBattleEndReactionCallback));
    }

    private bool CharacterHasBeenDefeated(int index, CharacterData characterData)
        => index == Constants.OpponentIndex && (
                !(characterData is null) && characterData.IsDefeated() || characterData is null
        ) || index == Constants.PlayerIndex && characterData.IsDefeated();

    private bool BattleHasEnded() => state.Equals(BattleState.OpponentDefeated) || state.Equals(BattleState.PlayerDefeated) || state.Equals(BattleState.Ran);

    private void EndBattle(Func<bool, bool> npcBattleEndReactionCallback)
    {
        ui.Close();
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
            int playerPokemonIndex = -1;
            yield return GetDecisionPlayer((battleOption, move, index) =>
            {
                playerBattleOption = battleOption;
                playerMove = move;
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

                if (BattleHasEnded())
                    break;
            }
            else
            {
                // Perform player actions first
                if (playerBattleOption == BattleOption.Fight)
                    yield return MoveCoroutine(Constants.PlayerIndex, Constants.OpponentIndex, playerMove);
                else if (playerBattleOption == BattleOption.PokemonSwitch)
                    yield return ChoosePokemon(Constants.PlayerIndex, playerPokemonIndex);
                //else if (playerBattleOption == BattleOption.Bag)
                // TODO
                else if (playerBattleOption == BattleOption.Run)
                    break;

                // Secondly, perform opponent actions
                if (opponentBattleOption == BattleOption.Fight)
                    yield return MoveCoroutine(Constants.OpponentIndex, Constants.PlayerIndex, opponentMove);
                else if (opponentBattleOption == BattleOption.PokemonSwitch)
                    yield return ChoosePokemon(Constants.OpponentIndex, opponentPokemonIndex);
                //else if (playerBattleOption == BattleOption.Bag)
                // TODO
            }
        }

        EndBattle(npcBattleEndReactionCallback);
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
        ui.OpenBattleMenu();
        BattleOption choosenOption = BattleOption.None;
        UserBattleOptionChooseEventHandler action = (BattleOption option) => choosenOption = option;
        UserChooseBattleOptionEvent += action;
        print("wait for play to choose option");
        yield return new WaitUntil(() => choosenOption != BattleOption.None);
        ui.CloseBattleMenu();
        UserChooseBattleOptionEvent -= action;
        callback(choosenOption);
    }

    private void GetDecisionOpponent(out BattleOption battleOption, out Move opponentMove)
    {
        // TODO: implement more intelligent decision making
        battleOption = BattleOption.Fight;
        opponentMove = GetMoveOpponent();
    }

    private IEnumerator GetDecisionPlayer(Action<BattleOption, Move, int> callback)
    {
        bool goBack = false;
        BattleOption battleOption;
        Move playerMove = null;
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
                //TODO
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
                playerMove = null;
                goBack = false;
                while (playerMove is null && !goBack)
                    yield return GetMovePlayer((choosenMove, back) =>
                    {
                        playerMove = choosenMove;
                        goBack = back;
                    });

                // Only continue with battle if player did not want to go back
                if (!goBack) break;
            }
        }

        callback(battleOption, playerMove, choosenPokemon);
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

        ui.OpenMoveSelection();

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
        ui.CloseMoveSelection();
        UserChooseMoveEvent -= action;

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
            if (!opponentData.pokemons[i].isFainted)
            {
                yield return ChoosePokemon(Constants.OpponentIndex, i);
                break;
            }
        }
    }

    private IEnumerator GetNextPokemonPlayer(Action<int, bool> callback, bool forceSelection = false)
    {
        playerBattleOption = BattleOption.None;
        ui.OpenPokemonSwitchSelection(forceSelection);

        int choosenPokemonIndex = -1;
        bool goBack = false;
        UserPokemonChooseEventHandler action =
            (int index, bool back) =>
            {
                choosenPokemonIndex = index;
                goBack = back;
            };
        UserChoosePokemonEvent += action;
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
        UserChoosePokemonEvent -= action;

        callback(choosenPokemonIndex, goBack);
    }

    private IEnumerator ChoosePokemon(int characterIndex, int pokemonIndex)
    {
        // TODO Animate pokemon retreat
        this.pokemonIndex[characterIndex] = pokemonIndex;

        if (characterIndex == Constants.PlayerIndex)
            unfaintedPlayerPokemons[opponentPokemon].Add(playerPokemon);

        // TODO Animate pokemon deployment
        ui.SwitchToPokemon(characterIndex, characterData[characterIndex].pokemons[pokemonIndex]);
        yield return null;
    }

    public void ChooseBattleMenuOption(BattleOption option) => UserChooseBattleOptionEvent?.Invoke(option);
    public void ChoosePlayerMove(Move move, bool goBack) => UserChooseMoveEvent?.Invoke(move, goBack);
    public void ChoosePlayerPokemon(int index, bool goBack) => UserChoosePokemonEvent?.Invoke(index, goBack);

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
            yield return new WaitWhile(ui.PlayMoveAnimation(attacker, move));
            yield return new WaitForSeconds(1f);
            yield return new WaitWhile(ui.PlayBlinkAnimation(target));

            // Deal damage
            damage = move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);
            targetFainted = targetPokemon.InflictDamage(damage);
            yield return new WaitWhile(ui.RefreshHPAnimated(target));
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
            //dialogBox.Close();
            bool attackerFainted = false;
            if (move.data.recoil > 0 && damage > 0)
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
        yield return new WaitWhile(ui.PlayFaintAnimation(characterIndex));

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
        foreach (Pokemon p in gainingPokemons)
            yield return GainXP(p, gainedXP);
        dialogBox.Close();
    }

    private IEnumerator GainXP(Pokemon pokemon, int xp)
    {
        yield return dialogBox.DrawText($"{pokemon.Name} erhält {xp} EP!", DialogBoxContinueMode.User);
        yield return ui.RefreshXPAnimated();
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
