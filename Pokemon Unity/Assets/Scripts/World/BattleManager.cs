using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CollectionExtensions;
using AYellowpaper;

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
    private ICharacterData playerData;
    private INPCData opponentData;
    private int[] pokemonIndex = new int[] { 0, 0 };
    private bool[] isDefeated = new bool[] { false, false };
    private bool opponentIsWild = false;
    private int runTryCount = 0;
    private Dictionary<IPokemon, HashSet<IPokemon>> unfaintedPlayerPokemons;
    private HashSet<Pokemon> evolvingPokemons;
    private int playerRunSuccessChance => playerPokemon.SpeedUnmodified >= opponentPokemon.SpeedUnmodified ? 256 :
        (playerPokemon.SpeedUnmodified * 128 / Math.Min(1, opponentPokemon.SpeedUnmodified) + 30 * runTryCount) % 256;

    private BattleOption playerBattleOption = BattleOption.None;
    private BattleOption opponentBattleOption = BattleOption.None;

    private ICharacterData[] characterData;

    private IPokemon wildPokemon;
    private IPokemon playerPokemon => GetActivePokemon(Constants.PlayerIndex);
    private IPokemon opponentPokemon => GetActivePokemon(Constants.OpponentIndex);

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

    private IPokemon GetActivePokemon(int character)
        => character == Constants.OpponentIndex && opponentIsWild ? wildPokemon : characterData[character].Pokemons[pokemonIndex[character]];

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
        playerPokemon.ResetWaitingStatusEffects();
        opponentPokemon.ResetWaitingStatusEffects();

        yield return ui.Open(playerData, playerPokemon, opponentPokemon);
        unfaintedPlayerPokemons = new Dictionary<IPokemon, HashSet<IPokemon>>();
        evolvingPokemons = new HashSet<Pokemon>();
    }

    public void StartNewEncounter(ICharacterData playerData, IPokemon wildPokemon, Func<bool, bool> encounterEndtionCallback)
    {
        print("StartNewEncounter");
        this.wildPokemon = wildPokemon;
        PlayerData.Instance.AddSeenPokemon(wildPokemon.Data);
        opponentIsWild = true;
        this.playerData = playerData;
        opponentData = null;
        characterData = new ICharacterData[] { this.playerData, null };
        BgmHandler.Instance.PlayOverlay(wildBattleMusicTrack);

        StartCoroutine(StartNewEncounterCoroutine(encounterEndtionCallback));
    }

    IEnumerator StartNewEncounterCoroutine(Func<bool, bool> encounterEndtionCallback)
    {
        yield return Reset();
        unfaintedPlayerPokemons[wildPokemon] = new HashSet<IPokemon>() { playerPokemon };
        yield return RoundCoroutine(encounterEndtionCallback);
    }

    public void StartNewBattle(ICharacterData playerData, INPCData opponentData, Func<bool, bool> npcBattleEndReactionCallback)
    {
        print("StartNewBattle");
        opponentIsWild = false;
        state = BattleState.None;
        this.playerData = playerData;
        this.opponentData = opponentData;
        characterData = new ICharacterData[] { this.playerData, this.opponentData };
        pokemonIndex[Constants.OpponentIndex] = opponentData.GetFirstAlivePokemonIndex();
        PlayerData.Instance.AddSeenPokemon(opponentPokemon.Data);
        BgmHandler.Instance.PlayOverlay(trainerBattleMusicTrack);

        StartCoroutine(StartNewBattleCoroutine(npcBattleEndReactionCallback));
    }

    IEnumerator StartNewBattleCoroutine(Func<bool, bool> npcBattleEndReactionCallback)
    {
        yield return Reset();

        foreach (Pokemon p in opponentData.Pokemons)
            unfaintedPlayerPokemons[p] = new HashSet<IPokemon>();
        unfaintedPlayerPokemons[opponentPokemon].Add(playerPokemon);

        yield return RoundCoroutine(npcBattleEndReactionCallback);
    }

    private bool CharacterHasBeenDefeated(int index, ICharacterData characterData)
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
            IMove opponentMove;
            int opponentPokemonIndex = -1;
            GetDecisionOpponent(out opponentBattleOption, out opponentMove);
            playerBattleOption = BattleOption.None;
            IMove playerMove = null;
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
        if (!item.Data.Value.UsableOnBattleOpponent)
        {
            // TODO: This is a workaround. It actually only needs to be called in case item has been used on player's active pokemon
            ui.Refresh(Constants.PlayerIndex);
            // TODO: do not break if item can be used on own pokemon and usage shall be animated
            yield break;
        }

        if (item.Data.Value.CatchesPokemon)
            yield return PlayerThrowBall(item);
    }

    private IEnumerator PlayerThrowBall(Item ball)
    {
        if (!PlayerData.Instance.TryTakeItem(ball))
        {
            Debug.LogError($"Tried to take ball {ball.Data.Value.Name} from player, but player does not own this item!");
            yield break;
        }
        dialogBox.DrawText($"{playerData.Name} wirft {ball.Data.Value.Name}!", DialogBoxContinueMode.External);

        yield return ui.PlayThrowAnimation();
        ui.HideOpponent();
        SfxHandler.Play(ballCloseSound);
        float rate = opponentPokemon.GetModifiedCatchRate(ball.Data.Value.CatchRateBonus);
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
        if (!PlayerData.Instance.HasCaughtPokemon(opponentPokemon.Data))
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
        {
            SfxHandler.Play(runSound);
            yield return dialogBox.DrawText($"Du bist entkommen!", DialogBoxContinueMode.User);
        }
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

    private void GetDecisionOpponent(out BattleOption battleOption, out IMove opponentMove)
    {
        // TODO: implement more intelligent decision making
        battleOption = BattleOption.Fight;
        opponentMove = GetMoveOpponent();
    }

    private IEnumerator GetDecisionPlayer(Action<BattleOption, IMove, Item, int> callback)
    {
        bool goBack = false;
        BattleOption battleOption;
        IMove chosenMove = null;
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

    private IMove GetMoveOpponent()
    {
        // TODO: implement more intelligent move choose
        return opponentPokemon.Moves[UnityEngine.Random.Range(0, opponentPokemon.Moves.Count)];
    }

    private IEnumerator GetMovePlayer(Action<IMove, bool> callback)
    {
        if(!playerPokemon.HasUsableMoves())
        {
            struggleMove.SetPokemon(playerPokemon);
            callback(struggleMove, false);
            yield break;
        }

        // Let player Choose move
        IMove choosenMove = null;
        bool goBack = false;
        ui.OpenMoveSelection((ISelectableUIElement selection, bool back) =>
        {
            goBack = back;
            choosenMove = selection is null ? null : playerPokemon.Moves[selection.GetIndex()];
        }, playerPokemon);
        print("wait for play to choose move");
        yield return new WaitUntil(() => choosenMove != null || goBack);
        ui.CloseMoveSelection();

        if (!(choosenMove is null) && choosenMove.Pp < 1)
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
        for (int i = 0; i < opponentData.Pokemons.Count; i++)
        {
            IPokemon pokemon = opponentData.Pokemons[i];
            if (!pokemon.IsFainted)
            {
                yield return dialogBox.DrawText($"{opponentData.Name} wird als nächstes {pokemon.SpeciesName} einsetzen.", DialogBoxContinueMode.User);
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

            if (playerData.Pokemons[choosenPokemonIndex].IsFainted)
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
            PlayerData.Instance.AddSeenPokemon(opponentPokemon.Data);

        IPokemon pokemon = GetActivePokemon(characterIndex);
        pokemon.ResetStatModifiers();
        pokemon.HealAllVolatileStatusEffects();
        pokemon.ResetWaitingStatusEffects();

        // TODO Animate pokemon deployment
        ui.SwitchToPokemon(characterIndex, pokemon);
        yield return new WaitForSeconds(BgmHandler.Instance.PlayMFX(pokemon.Data.Cry));
    }

    string GetUniqueIdentifier(IPokemon pokemon, IPokemon other, ICharacterData character)
        => pokemon.Name == other.Name ? (
            character is null ? (pokemon == opponentPokemon ? $"Wildes {pokemon.Name}" : pokemon.Name) : $"{character.NameGenitive} {pokemon.Name}"
        ) : pokemon.Name;

    private IEnumerator MoveCoroutine(int attacker, int target, IMove move)
    {
        bool canFight = true;

        yield return StatusEffectVolatileRoundTick(attacker, true, () => canFight &= false);
        yield return StatusEffectNonVolatileRoundTick(attacker, true, () => canFight &= false);
        if (!canFight)
            yield break;

        ICharacterData attackerCharacter = null;
        ICharacterData targetCharacter = null;
        if (target == Constants.PlayerIndex || !opponentIsWild)
            targetCharacter = characterData[target];
        if (attacker == Constants.PlayerIndex || !opponentIsWild)
            attackerCharacter = characterData[attacker];
        IPokemon attackerPokemon = GetActivePokemon(attacker);
        IPokemon targetPokemon = GetActivePokemon(target);
        move.DecrementPP();
        ui.RefreshMove(move);

        // Play attack animation
        string attackingPokemonIdentifier = GetUniqueIdentifier(attackerPokemon, targetPokemon, attackerCharacter);
        string targetPokemonIdentifier = GetUniqueIdentifier(targetPokemon, attackerPokemon, targetCharacter);

        // Display usage text
        string defaultUsageText = $"{attackingPokemonIdentifier} setzt {move.Data.Name} ein!";
        string usageText = null;
        if (move.Data.HasSpecialUsageText)
        {
            usageText = move.Data.SpecialUsageText;
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

        FailReason failReason = FailReason.None;
        if (move.TryHit(attackerPokemon, targetPokemon, out failReason))
        {
            // Attack hit successfully

            // Animation
            if (move.Data.UsesCryForSound)
                SfxHandler.Play(attackerPokemon.Data.Cry);
            else if (!(move.Data.Sound is null))
                SfxHandler.Play(move.Data.Sound);

            yield return ui.PlayMoveAnimation(attacker, move);

            yield return new WaitForSeconds(.5f);

            SfxHandler.Play(moveHitSounds[effectiveness]);
            if (!move.Data.DoesNotInflictDamage)
                yield return ui.PlayBlinkAnimation(target);

            // Deal damage
            damage = move.Data.Power < 1 ? 0 : move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);
            if (damage > 0)
            {
                yield return InflictDamage(target, damage);
                if (effectiveness != Effectiveness.Normal)
                    yield return dialogBox.DrawText(effectiveness, DialogBoxContinueMode.User);
                if (critical)
                    yield return dialogBox.DrawText($"Ein Volltreffer!", DialogBoxContinueMode.User);
            }

            yield return InflictStatModifiers(target, move.Data.StatModifiersTarget);
            if (UnityEngine.Random.value <= move.Data.StatusVolatileInflictedTargetChance)
                yield return InflictStatusEffect(target, move.Data.StatusVolatileInflictedTarget.Value, move.Data.RoundsBeforeFirstEffectVolatile);
            yield return InflictStatusEffect(target, move.Data.StatusNonVolatileInflictedTarget.Value, move.Data.RoundsBeforeFirstEffectNonVolatile);

            if (UnityEngine.Random.value <= move.Data.StatusNonVolatileInflictedTargetChance)
                yield return InflictStatusEffect(target, move.Data.StatusVolatileInflictedTarget.Value, move.Data.RoundsBeforeFirstEffectVolatile);
            yield return InflictStatusEffect(attacker, move.Data.StatusNonVolatileInflictedSelf.Value, move.Data.RoundsBeforeFirstEffectNonVolatile);
            yield return InflictStatModifiers(attacker, move.Data.StatModifiersSelf);
        }
        else
        {
            // Attack failed
            if (failReason == FailReason.NoEffect)
                yield return dialogBox.DrawText($"Es hat keinen Effekt!", DialogBoxContinueMode.User);
            else
                yield return dialogBox.DrawText($"Attacke von {attackingPokemonIdentifier} geht daneben!", DialogBoxContinueMode.User);
        }

        if (targetPokemon.IsFainted)
            // target pokemon fainted
            yield return Faint(target, targetPokemonIdentifier);
        else
        {
            if (move.Data.Recoil > 0 && damage > 0)
            {
                yield return dialogBox.DrawText($"{attackingPokemonIdentifier} wird durch Rückstoß getroffen!", DialogBoxContinueMode.Automatic);
                yield return new WaitForSeconds(1f);
                SfxHandler.Play(moveHitSounds[Effectiveness.Normal]);
                yield return ui.PlayBlinkAnimation(target);
                int recoilDamage = (int)(damage * move.Data.Recoil);
                yield return InflictDamage(attacker, recoilDamage);
                yield return ui.RefreshHPAnimated(attacker);
            }

            if (attackerPokemon.IsFainted)
                yield return Faint(attacker, attackingPokemonIdentifier);
            else
                yield return InflictNextWaitingStatusEffect(attacker);
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

        IPokemon targetPokemon = GetActivePokemon(target);
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

    private IEnumerator InflictNextWaitingStatusEffect(int targetIndex)
    {
        // TODO separate non volatile and volatile effects
        IPokemon targetPokemon = GetActivePokemon(targetIndex);
        yield return InflictStatusEffect(targetIndex, targetPokemon.GetNextNonVolatileStatusEffectFromWaitingList());
        yield return InflictStatusEffect(targetIndex, targetPokemon.GetNextVolatileStatusEffectFromWaitingList());
    }

    private IEnumerator InflictStatusEffect(int targetIndex, IStatusEffectData statusEffect, int roundsBeforeFirstEffect = 0)
    {
        // TODO Animation
        IPokemon targetPokemon = GetActivePokemon(targetIndex);
        if (statusEffect is null ||
            statusEffect.IsNonVolatile && !(targetPokemon.StatusEffectNonVolatile is null) ||
            statusEffect.IsVolatile && targetPokemon.HasStatusEffectVolatile(statusEffect)
        )
            yield break;

        string inflictionText;
        bool willBeInflictedNow = roundsBeforeFirstEffect < 1;
        if (!willBeInflictedNow)
        {
            targetPokemon.InflictWaitingStatusEffect(new WaitingStatusEffect(statusEffect, roundsBeforeFirstEffect));
            inflictionText = statusEffect.WaitForEffectText;
        }
        else
            inflictionText = statusEffect.InflictionText;

        dialogBox.DrawText(
            TextKeyManager.ReplaceKey(
                TextKeyManager.TextKeyPokemon,
                inflictionText,
                targetPokemon.Name), DialogBoxContinueMode.External);

        if(willBeInflictedNow)
        {
            SfxHandler.Play(statusEffect.Sound);
            yield return ui.PlayInflictStatusAnimation(targetIndex);
            yield return new WaitForSeconds(2f);

            targetPokemon.InflictStatusEffect(statusEffect);
            ui.Refresh(targetIndex);
        }

        yield return new WaitForSeconds(2f);
        dialogBox.Close();
    }

    private IEnumerator StatusEffectNonVolatileRoundTick(int target, bool beforeMove, Action preventMoveCallback = null)
    {
        IPokemon targetPokemon = GetActivePokemon(target);
        yield return StatusEffectRoundTick(targetPokemon.StatusEffectNonVolatile, target, targetPokemon, beforeMove, preventMoveCallback);
    }

    private IEnumerator StatusEffectVolatileRoundTick(int target, bool beforeMove, Action preventMoveCallback = null)
    {
        IPokemon targetPokemon = GetActivePokemon(target);
        StatusEffect[] statusList = new StatusEffect[targetPokemon.StatusEffectsVolatile.Count];
        targetPokemon.StatusEffectsVolatile.CopyTo(statusList);
        foreach (StatusEffect s in statusList)
            yield return StatusEffectRoundTick(s, target, targetPokemon, beforeMove, preventMoveCallback);
    }

    private IEnumerator StatusEffectRoundTick(IStatusEffect statusEffect, int targetIndex, IPokemon targetPokemon, bool beforeMove, Action preventMoveCallback)
    {
        if (statusEffect is null)
            yield break;

        int selfDamage = statusEffect.Data.GetDamageAgainstSelf(targetPokemon);
        int damage = statusEffect.Data.DamagePerRoundAbsolute +
            Mathf.RoundToInt(statusEffect.Data.DamagePerRoundRelativeToMaxHp * targetPokemon.MaxHp) +
            selfDamage;

        if (beforeMove && !statusEffect.Data.TakesEffectBeforeMoves ||
            !beforeMove && statusEffect.Data.TakesEffectBeforeMoves ||
            damage < 1 && !statusEffect.Data.PreventsMove
        )
            yield break;

        bool lifeTimeEnds = !statusEffect.Data.LivesForever && statusEffect.LifeTime < 1;
        if (lifeTimeEnds)
            yield return HealStatus(targetIndex, statusEffect);
        else if (statusEffect.Data.NotificationPerRoundText != "")
            yield return dialogBox.DrawText(TextKeyManager.ReplaceKey(
                TextKeyManager.TextKeyPokemon, statusEffect.Data.NotificationPerRoundText, targetPokemon.Name), closeAfterFinish: true);

        if ((!lifeTimeEnds || statusEffect.Data.TakesEffectOnceWhenLifeTimeEnded) && UnityEngine.Random.value <= statusEffect.Data.Chance)
        {
            if (!lifeTimeEnds)
            {
                dialogBox.DrawText(TextKeyManager.ReplaceKey(
                    TextKeyManager.TextKeyPokemon, statusEffect.Data.EffectPerRoundText, targetPokemon.Name),
                    DialogBoxContinueMode.External);
                yield return new WaitForSeconds(1f);
            }

            if(selfDamage > 0)
            {
                SfxHandler.Play(moveHitSounds[Effectiveness.Normal]);
                yield return ui.PlayBlinkAnimation(targetIndex);
            }
            else
            {
                SfxHandler.Play(statusEffect.Data.Sound);
                yield return ui.PlayInflictStatusAnimation(targetIndex);
            }
            dialogBox.Close();

            if (damage > 0)
            {
                yield return InflictDamage(targetIndex, damage);
                if (targetPokemon.IsFainted)
                    yield return Faint(targetIndex, targetPokemon.Name);
            }

            if (statusEffect.Data.PreventsMove)
                preventMoveCallback?.Invoke();

            statusEffect.LifeTime--;
        }
    }

    private IEnumerator HealStatus(int target, IStatusEffect statusEffect)
    {
        IPokemon targetPokemon = GetActivePokemon(target);
        yield return dialogBox.DrawText(TextKeyManager.ReplaceKey(
                    TextKeyManager.TextKeyPokemon, statusEffect.Data.EndOfLifeText, targetPokemon.Name), closeAfterFinish: true);
        targetPokemon.HealStatus(statusEffect);
        ui.Refresh(target);
    }

    private IEnumerator Faint(int characterIndex, string pokemonIdentifier)
    {
        dialogBox.DrawText($"{pokemonIdentifier} wurde besiegt!", DialogBoxContinueMode.External);

        yield return new WaitForSeconds(BgmHandler.Instance.PlayMFX(GetActivePokemon(characterIndex).Data.FaintCry));
        SfxHandler.Play(faintSound);
        yield return ui.PlayFaintAnimation(characterIndex);

        if (opponentIsWild)
            BgmHandler.Instance.PlayOverlay(wildVictoryMusicTrack);

        if (characterIndex == Constants.PlayerIndex)
            foreach (HashSet<IPokemon> set in unfaintedPlayerPokemons.Values)
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
        HashSet<IPokemon> gainingPokemons = unfaintedPlayerPokemons[opponentPokemon];
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
        ICharacterData winner = characterData[winnerIndex];
        ICharacterData loser = characterData[loserIndex];
        bool winnerIsTrainer = !(winner is null);
        bool loserIsTrainer = !(loser is null);

        if (!opponentIsWild)
            ui.MakeOpponentAppear();
        if (loserIsTrainer)
        {
            BgmHandler.Instance.PlayOverlay(trainerVictoryMusicTrack);
            yield return dialogBox.DrawText($"{loser.Name} wurde besiegt!", DialogBoxContinueMode.User);
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
