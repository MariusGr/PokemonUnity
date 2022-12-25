using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CollectionExtensions;

public class BattleManager : ManagerWithPokemonManager, IBattleManager
{
    private enum BattleState
    {
        None,
        ChoosingMove,
        BattleMenu,
        Ran,
        OpponentDefeated,
        OpponentCaught,
        PlayerDefeated,
    }

    [SerializeField] Move struggleMove;
    [SerializeField] AudioClip trainerBattleMusicTrack;
    [SerializeField] AudioClip wildBattleMusicTrack;
    [SerializeField] AudioClip wildVictoryMusicTrack;
    [SerializeField] AudioClip trainerVictoryMusicTrack;
    [SerializeField] AudioClip pokemonCaughtMusicTrack;
    [SerializeField] AudioClip statUpSound;
    [SerializeField] AudioClip statDownSound;
    [SerializeField] AudioClip faintSound;
    [SerializeField] AudioClip xpGainSound;
    [SerializeField] AudioClip ballCloseSound;
    [SerializeField] AudioClip ballOpenSound;
    [SerializeField] AudioClip runSound;
    [SerializeField] InspectorFriendlySerializableDictionary<Effectiveness, AudioClip> moveHitSounds;

    private BattleState state;
    private CharacterData playerData;
    private NPCData opponentData;
    private int[] pokemonIndex = new int[] { 0, 0 };
    private bool[] isDefeated = new bool[] { false, false };
    private bool opponentIsWild = false;
    private int runTryCount = 0;
    private Dictionary<Pokemon, HashSet<Pokemon>> unfaintedPlayerPokemons;
    private HashSet<Pokemon> evolvingPokemons;
    private int playerRunSuccessChance => playerPokemon.speedUnmodified >= opponentPokemon.speedUnmodified ? 256 :
        (playerPokemon.speedUnmodified * 128 / Math.Min(1, opponentPokemon.speedUnmodified) + 30 * runTryCount) % 256;

    private BattleOption playerBattleOption = BattleOption.None;
    private BattleOption opponentBattleOption = BattleOption.None;

    private CharacterData[] characterData;

    private Pokemon wildPokemon;
    private Pokemon playerPokemon => GetActivePokemon(Constants.PlayerIndex);
    private Pokemon opponentPokemon => GetActivePokemon(Constants.OpponentIndex);

    private IBattleUI ui;
    private IEvolutionManager evolutionManager;

    public BattleManager() => Services.Register(this as IBattleManager);
    public bool OpponentIsWild() => opponentIsWild;

    void Awake()
    {
        ui = Services.Get<IBattleUI>();
        evolutionManager = Services.Get<IEvolutionManager>();
        Initialize();
    }

    private Pokemon GetActivePokemon(int character)
        => character == Constants.OpponentIndex && opponentIsWild ? wildPokemon : characterData[character].pokemons[pokemonIndex[character]];

    private IEnumerator Reset()
    {
        state = BattleState.None;
        runTryCount = 0;
        state = BattleState.ChoosingMove;
        pokemonIndex[Constants.PlayerIndex] = playerData.GetFirstAlivePokemonIndex();
        isDefeated = new bool[] { false, false };

        playerPokemon.ResetStatModifiers();
        opponentPokemon.ResetStatModifiers();
        playerPokemon.HealAllVolatileStatusEffects();
        opponentPokemon.HealAllVolatileStatusEffects();

        yield return ui.Open(playerData, playerPokemon, opponentPokemon);
        unfaintedPlayerPokemons = new Dictionary<Pokemon, HashSet<Pokemon>>();
        evolvingPokemons = new HashSet<Pokemon>();
    }

    public void StartNewEncounter(CharacterData playerData, Pokemon wildPokemon, Func<bool, bool> encounterEndtionCallback)
    {
        print("StartNewEncounter");
        this.wildPokemon = wildPokemon;
        PlayerData.Instance.AddSeenPokemon(wildPokemon.data);
        opponentIsWild = true;
        this.playerData = playerData;
        opponentData = null;
        characterData = new CharacterData[] { this.playerData, null };
        BgmHandler.Instance.PlayOverlay(wildBattleMusicTrack);

        StartCoroutine(StartNewEncounterCoroutine(encounterEndtionCallback));
    }

    IEnumerator StartNewEncounterCoroutine(Func<bool, bool> encounterEndtionCallback)
    {
        yield return Reset();
        unfaintedPlayerPokemons[wildPokemon] = new HashSet<Pokemon>() { playerPokemon };
        yield return RoundCoroutine(encounterEndtionCallback);
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
        PlayerData.Instance.AddSeenPokemon(opponentPokemon.data);
        BgmHandler.Instance.PlayOverlay(trainerBattleMusicTrack);

        StartCoroutine(StartNewBattleCoroutine(npcBattleEndReactionCallback));
    }

    IEnumerator StartNewBattleCoroutine(Func<bool, bool> npcBattleEndReactionCallback)
    {
        yield return Reset();

        foreach (Pokemon p in this.opponentData.pokemons)
            unfaintedPlayerPokemons[p] = new HashSet<Pokemon>();
        unfaintedPlayerPokemons[opponentPokemon].Add(playerPokemon);

        yield return RoundCoroutine(npcBattleEndReactionCallback);
    }

    private bool CharacterHasBeenDefeated(int index, CharacterData characterData)
        => index == Constants.OpponentIndex && (
                !(characterData is null) && characterData.IsDefeated() || characterData is null
        ) || index == Constants.PlayerIndex && characterData.IsDefeated();

    private bool BattleHasEnded() => state.Equals(BattleState.OpponentDefeated) ||
        state.Equals(BattleState.PlayerDefeated) ||
        state.Equals(BattleState.Ran) ||
        state.Equals(BattleState.OpponentCaught);

    private IEnumerator EndBattle(Func<bool, bool> npcBattleEndReactionCallback)
    {
        // TODO: ui close animation
        dialogBox.Close();
        yield return ui.Close();

        // Evolve pokemon if any leveled up enough
        foreach (Pokemon p in evolvingPokemons)
            yield return evolutionManager.EvolutionCoroutine(p);

        BgmHandler.Instance.ResumeMain();
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

            IEnumerator opponentCoroutine = MoveCoroutine(Constants.OpponentIndex, Constants.PlayerIndex, opponentMove);

            // Both trainers will fight -> priority decided by attack speed
            if (playerBattleOption == BattleOption.Fight && opponentBattleOption == BattleOption.Fight)
            {
                IEnumerator playerCoroutine = MoveCoroutine(Constants.PlayerIndex, Constants.OpponentIndex, playerMove);

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
                {
                    yield return PlayerUseItem(playerItem);
                    // Battle has possibly been ended by catching opponent pokemon
                    if (BattleHasEnded())
                        break;
                }
                else if (playerBattleOption == BattleOption.Run)
                    break;

                // Secondly, perform opponent actions

                if (opponentBattleOption == BattleOption.Fight)
                {
                        yield return opponentCoroutine;
                }
                else if (opponentBattleOption == BattleOption.PokemonSwitch)
                    yield return ChoosePokemon(Constants.OpponentIndex, opponentPokemonIndex);
                //else if (playerBattleOption == BattleOption.Bag)
                    //TODO
            }

            yield return StatusEffectVolatileRoundTick(Constants.OpponentIndex, false);
            yield return StatusEffectNonVolatileRoundTick(Constants.OpponentIndex, false);
            yield return StatusEffectVolatileRoundTick(Constants.PlayerIndex, false);
            yield return StatusEffectNonVolatileRoundTick(Constants.PlayerIndex, false);

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
            ui.Refresh(Constants.PlayerIndex);
            // TODO: do not break if item can be used on own pokemon and usage shall be animated
            yield break;
        }

        if (item.data.catchesPokemon)
            yield return PlayerThrowBall(item);
    }

    private IEnumerator PlayerThrowBall(Item ball)
    {
        if (!PlayerData.Instance.TryTakeItem(ball))
        {
            Debug.LogError($"Tried to take ball {ball.data.fullName} from player, but player does not own this item!");
            yield break;
        }
        dialogBox.DrawText($"{playerData.name} wirft {ball.data.fullName}!", DialogBoxContinueMode.External);

        yield return ui.PlayThrowAnimation();
        ui.HideOpponent();
        SfxHandler.Play(ballCloseSound);
        float rate = opponentPokemon.GetModifiedCatchRate(ball.data.catchRateBonus);
        int shakeProbability = Mathf.RoundToInt(1048560f / Mathf.Sqrt(Mathf.Sqrt(16711680f / rate)));

        for (int i = 0; i < 4; i++)
        {
            yield return ui.PlayShakeAnimation();

            int randomValue = UnityEngine.Random.Range(0, 65536);
            if (randomValue >= shakeProbability)
            {
                // fail
                print($"Shake #{i}: Catch failed, {randomValue} >= {shakeProbability}");
                ui.ShowOpponent();
                ui.HidePokeBallAnimation();
                SfxHandler.Play(ballOpenSound);
                yield return dialogBox.DrawText($"{opponentPokemon.Name} hat sich wieder befreit!", DialogBoxContinueMode.User);
                yield break;
            }

            // success
            print($"Shake #{i}: Catch success, {randomValue} < {shakeProbability}");
            yield return new WaitForSeconds(1f);
        }

        // caught
        print("Caught!");
        BgmHandler.Instance.PlayMFX(pokemonCaughtMusicTrack);
        dialogBox.DrawText($"{opponentPokemon.Name} wurde gefangen!", DialogBoxContinueMode.External);
        yield return new WaitForSeconds(5f);
        BgmHandler.Instance.PlayOverlay(wildVictoryMusicTrack);
        if (!PlayerData.Instance.HasCaughtPokemon(opponentPokemon.data))
            yield return dialogBox.DrawText($"Für {opponentPokemon.Name} wird einer neuer Eintrag im Pokedex angelegt.");

        if (PlayerData.Instance.PartyIsFull())
            yield return dialogBox.DrawText($"{opponentPokemon.Name} wurde in der Box deponiert!");
        PlayerData.Instance.CatchPokemon(opponentPokemon);
        // TODO Nickname geben
        state = BattleState.OpponentCaught;
        yield break;
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
        print(playerRunSuccessChance);
        bool success = forceSuccess || opponentIsWild && UnityEngine.Random.Range(0, 256) < playerRunSuccessChance;

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
                if (!opponentIsWild)
                {
                    yield return dialogBox.DrawText("Du kannst nicht aus einem Trainerkampf fliehen!", closeAfterFinish: true);
                    continue;
                }

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
            yield return GetNextPokemonPlayer();
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
        for (int i = 0; i < opponentData.pokemons.Count; i++)
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

        if (characterIndex == Constants.OpponentIndex)
            PlayerData.Instance.AddSeenPokemon(opponentPokemon.data);

        Pokemon pokemon = GetActivePokemon(characterIndex);
        pokemon.ResetStatModifiers();
        pokemon.HealAllVolatileStatusEffects();

        // TODO Animate pokemon deployment
        ui.SwitchToPokemon(characterIndex, pokemon);
        yield return new WaitForSeconds(BgmHandler.Instance.PlayMFX(pokemon.data.cry));
    }

    string GetUniqueIdentifier(Pokemon pokemon, Pokemon other, CharacterData character)
        => pokemon.Name == other.Name ? (
            character is null ? (pokemon == opponentPokemon ? $"Wildes {pokemon.Name}" : pokemon.Name) : $"{character.nameGenitive} {pokemon.Name}"
        ) : pokemon.Name;

    private IEnumerator MoveCoroutine(int attacker, int target, Move move)
    {
        bool canFight = true;
        yield return StatusEffectVolatileRoundTick(attacker, true, () => canFight &= false);
        yield return StatusEffectNonVolatileRoundTick(attacker, true, () => canFight &= false);
        if (!canFight)
            yield break;

        CharacterData attackerCharacter = null;
        CharacterData targetCharacter = null;
        if (target == Constants.PlayerIndex || !opponentIsWild)
            targetCharacter = characterData[target];
        if (attacker == Constants.PlayerIndex || !opponentIsWild)
            attackerCharacter = characterData[attacker];
        Pokemon attackerPokemon = GetActivePokemon(attacker);
        Pokemon targetPokemon = GetActivePokemon(target);
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
        int damage = 0;
        Effectiveness effectiveness = Effectiveness.Normal;

        Move.FailReason failReason = Move.FailReason.None;
        if (move.TryHit(attackerPokemon, targetPokemon, out failReason))
        {
            // Attack hit successfully

            // Animation
            if (move.data.usesCryForSound)
                SfxHandler.Play(attackerPokemon.data.cry);
            else if (!(move.data.sound is null))
                SfxHandler.Play(move.data.sound);

            yield return ui.PlayMoveAnimation(attacker, move);

            yield return new WaitForSeconds(.5f);

            SfxHandler.Play(moveHitSounds[effectiveness]);
            if (!move.data.doesNotInflictDamage)
                yield return ui.PlayBlinkAnimation(target);

            // Deal damage
            damage = move.data.power < 1 ? 0 : move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);
            if (damage > 0)
            {
                yield return InflictDamage(target, damage);
                if (effectiveness != Effectiveness.Normal)
                    yield return dialogBox.DrawText(effectiveness, DialogBoxContinueMode.User);
                if (critical)
                    yield return dialogBox.DrawText($"Ein Volltreffer!", DialogBoxContinueMode.User);
            }

            yield return InflictStatModifiers(target, move.data.statModifiersTarget);
            yield return InflictStatusEffect(target, move.data.statusVolatileInflictedTarget);
            yield return InflictStatusEffect(target, move.data.statusNonVolatileInflictedTarget);

            yield return InflictStatusEffect(target, move.data.statusVolatileInflictedTarget);
            yield return InflictStatusEffect(attacker, move.data.statusNonVolatileInflictedSelf);
            yield return InflictStatModifiers(attacker, move.data.statModifiersSelf);
        }
        else
        {
            // Attack failed
            if (failReason == Move.FailReason.NoEffect)
                yield return dialogBox.DrawText($"Es hat keinen Effekt!", DialogBoxContinueMode.User);
            else
                yield return dialogBox.DrawText($"Attacke von {attackingPokemonIdentifier} geht daneben!", DialogBoxContinueMode.User);
        }

        if (targetPokemon.isFainted)
            // target pokemon fainted
            yield return Faint(target, targetPokemonIdentifier);
        else
        {
            if (move.data.recoil > 0 && damage > 0)
            {
                yield return dialogBox.DrawText($"{attackingPokemonIdentifier} wird durch Rückstoß getroffen!", DialogBoxContinueMode.Automatic);
                yield return new WaitForSeconds(1f);
                SfxHandler.Play(moveHitSounds[Effectiveness.Normal]);
                yield return ui.PlayBlinkAnimation(target);
                int recoilDamage = (int)(damage * move.data.recoil);
                yield return InflictDamage(attacker, recoilDamage);
                yield return ui.RefreshHPAnimated(attacker);
            }

            if (attackerPokemon.isFainted)
                yield return Faint(attacker, attackingPokemonIdentifier);
        }

        dialogBox.Close();
    }

    private IEnumerator InflictDamage(int target, int damage)
    {
        GetActivePokemon(target).InflictDamage(damage);
        yield return ui.RefreshHPAnimated(target);
    }

    private IEnumerator InflictStatModifiers(int target, Dictionary<Stat, int> statModifiers)
    {
        foreach (KeyValuePair<Stat, int> entry in statModifiers)
            yield return InflictStatModifier(target, entry.Key, entry.Value);
    }

    private IEnumerator InflictStatModifier(int target, Stat stat, int amount)
    {
        if (amount == 0)
            yield break;

        if (amount > 0)
        {
            SfxHandler.Play(statUpSound);
            yield return ui.PlayStatUpAnimation(target);
        }
        else
        {
            SfxHandler.Play(statDownSound);
            yield return ui.PlayStatDownAnimation(target);
        }

        Pokemon targetPokemon = GetActivePokemon(target);
        int result = targetPokemon.InflictStatModifier(stat, amount);
        string statString = TextKeyManager.statToString[stat];
        if (result == 0)
            yield return dialogBox.DrawText(
                $"{statString} von {targetPokemon.Name} {TextKeyManager.GetStatModificationDescription(amount)}!", closeAfterFinish: true);
        else if (result < 0)
            yield return dialogBox.DrawText(
                $"{statString} von {targetPokemon.Name} kann nicht weiter fallen!", closeAfterFinish: true);
        else
            yield return dialogBox.DrawText(
                $"{statString} von {targetPokemon.Name} kann nicht weiter steigen!", closeAfterFinish: true);
    }

    private IEnumerator InflictStatusEffect(int targetIndex, StatusEffectData statusEffect)
    {
        // TODO Animation
        Pokemon targetPokemon = GetActivePokemon(targetIndex);
        if (statusEffect is null ||
            statusEffect.isNonVolatile && !(targetPokemon.statusEffectNonVolatile is null) ||
            statusEffect.isVolatile && targetPokemon.HasStatusEffectVolatile(statusEffect)
        )
            yield break;

        dialogBox.DrawText(
            TextKeyManager.ReplaceKey(TextKeyManager.TextKeyPokemon, statusEffect.inflictionText, targetPokemon.Name), DialogBoxContinueMode.External);

        SfxHandler.Play(statusEffect.sound);
        yield return ui.PlayInflictStatusAnimation(targetIndex);
        yield return new WaitForSeconds(2f);
        dialogBox.Close();

        targetPokemon.InflictStatusEffect(statusEffect);
        ui.Refresh(targetIndex);
    }

    private IEnumerator StatusEffectNonVolatileRoundTick(int target, bool beforeMove, Action preventMoveCallback = null)
    {
        Pokemon targetPokemon = GetActivePokemon(target);
        yield return StatusEffectRoundTick(targetPokemon.statusEffectNonVolatile, target, targetPokemon, beforeMove, preventMoveCallback);
    }

    private IEnumerator StatusEffectVolatileRoundTick(int target, bool beforeMove, Action preventMoveCallback = null)
    {
        Pokemon targetPokemon = GetActivePokemon(target);
        StatusEffect[] statusList = new StatusEffect[targetPokemon.statusEffectsVolatile.Count];
        targetPokemon.statusEffectsVolatile.CopyTo(statusList);
        foreach (StatusEffect s in statusList)
            yield return StatusEffectRoundTick(s, target, targetPokemon, beforeMove, preventMoveCallback);
    }

    private IEnumerator StatusEffectRoundTick(StatusEffect statusEffect, int targetIndex, Pokemon targetPokemon, bool beforeMove, Action preventMoveCallback)
    {
        int selfDamage = statusEffect is null ? 0 : statusEffect.data.GetDamageAgainstSelf(targetPokemon);
        int damage = statusEffect is null ? 0 :
            statusEffect.data.damagePerRoundAbsolute +
            Mathf.RoundToInt(statusEffect.data.damagePerRoundRelativeToMaxHp * targetPokemon.maxHp) +
            selfDamage;

        if (statusEffect is null ||
            beforeMove && !statusEffect.data.takesEffectBeforeMoves ||
            !beforeMove && statusEffect.data.takesEffectBeforeMoves ||
            damage < 1 && !statusEffect.data.preventsMove
        )
            yield break;

        bool lifeTimeEnds = !statusEffect.data.livesForever && statusEffect.lifeTime < 1;
        if (lifeTimeEnds)
            yield return HealStatus(targetIndex, statusEffect);
        else if (statusEffect.data.notificationPerRoundText != "")
            yield return dialogBox.DrawText(TextKeyManager.ReplaceKey(
                TextKeyManager.TextKeyPokemon, statusEffect.data.notificationPerRoundText, targetPokemon.Name), closeAfterFinish: true);

        if ((!lifeTimeEnds || statusEffect.data.takesEffectOnceWhenLifeTimeEnded) && UnityEngine.Random.value <= statusEffect.data.chance)
        {
            if (!lifeTimeEnds)
                dialogBox.DrawText(TextKeyManager.ReplaceKey(
                    TextKeyManager.TextKeyPokemon, statusEffect.data.effectPerRoundText, targetPokemon.Name),
                    DialogBoxContinueMode.External);

            if(selfDamage > 0)
            {
                SfxHandler.Play(moveHitSounds[Effectiveness.Normal]);
                yield return ui.PlayBlinkAnimation(targetIndex);
            }
            else
            {
                SfxHandler.Play(statusEffect.data.sound);
                yield return ui.PlayInflictStatusAnimation(targetIndex);
            }
            dialogBox.Close();

            if (damage > 0)
            {
                yield return InflictDamage(targetIndex, damage);
                if (targetPokemon.isFainted)
                    yield return Faint(targetIndex, targetPokemon.Name);
            }

            if (statusEffect.data.preventsMove)
                preventMoveCallback?.Invoke();

            statusEffect.lifeTime--;
        }
    }

    private IEnumerator HealStatus(int target, StatusEffect statusEffect)
    {
        Pokemon targetPokemon = GetActivePokemon(target);
        yield return dialogBox.DrawText(TextKeyManager.ReplaceKey(
                    TextKeyManager.TextKeyPokemon, statusEffect.data.endOfLifeText, targetPokemon.Name), closeAfterFinish: true);
        targetPokemon.HealStatus(statusEffect);
        ui.Refresh(target);
    }

    private IEnumerator Faint(int characterIndex, string pokemonIdentifier)
    {
        dialogBox.DrawText($"{pokemonIdentifier} wurde besiegt!", DialogBoxContinueMode.External);

        yield return new WaitForSeconds(BgmHandler.Instance.PlayMFX(GetActivePokemon(characterIndex).data.faintCry));
        SfxHandler.Play(faintSound);
        yield return ui.PlayFaintAnimation(characterIndex);

        if (opponentIsWild)
            BgmHandler.Instance.PlayOverlay(wildVictoryMusicTrack);

        if (characterIndex == Constants.PlayerIndex)
            foreach (HashSet<Pokemon> set in unfaintedPlayerPokemons.Values)
                set.Remove(playerPokemon);
        else
            yield return DistributePlayerXP();

        bool characterHasBeenDefeated = CharacterHasBeenDefeated(characterIndex, characterData[characterIndex]);
        isDefeated[characterIndex] = characterHasBeenDefeated;
        if (!characterHasBeenDefeated)
            yield return ChooseNextPokemon(characterIndex);
        else
            yield return Defeat(characterIndex);

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
            AudioSource source = SfxHandler.Play(xpGainSound);
            float time = Time.time;
            yield return ui.RefreshXPAnimated();
            yield return new WaitForSeconds(Mathf.Max(0, .4f - (Time.time - time)));
            source.Stop();
            if (!pokemon.WillGrowLevel())
                break;
            yield return pokemonManager.GrowLevel(pokemon, ui.RefreshPlayerStats);
            if (pokemon.WillEvolve())
                evolvingPokemons.Add(pokemon);
            // TODO: Show stats
            ui.ResetXP();
        }
    }

    private IEnumerator Defeat(int loserIndex)
    {
        int winnerIndex = loserIndex > 0 ? 0 : 1;
        CharacterData winner = characterData[winnerIndex];
        CharacterData loser = characterData[loserIndex];
        bool winnerIsTrainer = !(winner is null);
        bool loserIsTrainer = !(loser is null);

        if (!opponentIsWild)
            ui.MakeOpponentAppear();
        if (loserIsTrainer)
        {
            BgmHandler.Instance.PlayOverlay(trainerVictoryMusicTrack);
            yield return dialogBox.DrawText($"{loser.name} wurde besiegt!", DialogBoxContinueMode.User);
        }

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

            state = BattleState.PlayerDefeated;
            yield return PlayerCharacter.Instance.Defeat();
        }
    }
}
