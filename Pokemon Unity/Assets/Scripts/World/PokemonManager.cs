using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonManager : ManagerWithDialogBox, IPokemonManager
{
    public static PokemonManager Instance;

    [SerializeField] IMoveSelectionUI moveSelectionUI;
    [SerializeField] AudioClip xpFullSound;
    [SerializeField] AudioClip levelGainMusic;
    [SerializeField] AudioClip healSound;

    public PokemonManager()
    {
        Instance = this;
        Services.Register(this as IPokemonManager);
    }

    private void Awake() => Initialize();

    public IEnumerator GrowLevel(IPokemon pokemon, System.Action<bool> uiRefreshCallback)
    {
        SfxHandler.Play(xpFullSound);
        yield return new WaitForSeconds(xpFullSound.length);

        pokemon.GrowLevel();
        uiRefreshCallback(false);

        BgmHandler.Instance.PlayMFX(levelGainMusic);
        yield return dialogBox.DrawText($"{pokemon.Name} erreicht Level {pokemon.Level}!", DialogBoxContinueMode.User, closeAfterFinish: true);

        IMoveData move;
        if (pokemon.Data.GetMoveLearnedAtLevel(pokemon.Level, out move))
            yield return TryLearnMove(pokemon, move);
    }

    public IEnumerator TryLearnMove(IPokemon pokemon, IMoveData move)
    {
        if (pokemon.Moves.Count >= 4)
        {
            while (true)
            {
                // Should an move be forgotten?
                yield return dialogBox.DrawText($"{pokemon.Name} will {move.Name} erlernen.", DialogBoxContinueMode.User);
                yield return dialogBox.DrawText($"Aber {pokemon.Name} kann keine weiteren Attacken mehr erlernen!", DialogBoxContinueMode.User);
                yield return dialogBox.DrawChoiceBox($"Soll {pokemon.Name} eine Attacke vergessen um {move.Name} zu erlernen?");
                if (dialogBox.GetChosenIndex() == 0)
                {
                    // Forget move

                    // Let player choose move
                    dialogBox.Close();
                    IMove choosenMove = null;
                    bool goBack = false;
                    moveSelectionUI.Open((ISelectableUIElement selection, bool back) =>
                    {
                        choosenMove = selection is null ? null : pokemon.Moves[selection.GetIndex()];
                        goBack = back;
                    });

                    print("wait for play to choose move");
                    yield return new WaitUntil(() => choosenMove != null || goBack);
                    moveSelectionUI.Close();

                    if (!goBack)
                    {
                        yield return dialogBox.DrawText($"Und...", DialogBoxContinueMode.User);
                        yield return dialogBox.DrawText($"Zack!", DialogBoxContinueMode.User);
                        yield return dialogBox.DrawText($"{pokemon.Name} hat {choosenMove.Data.Name} vergessen, und...", DialogBoxContinueMode.User);
                        pokemon.ReplaceMove(choosenMove, move);
                        break;
                    }
                }
                else
                {
                    // Dont forget move
                    yield return dialogBox.DrawText($"{move.Name} wurde nicht erlernt.", DialogBoxContinueMode.User);
                    yield break;
                }
            }
        }
        else
            pokemon.AddMove(move);
        yield return dialogBox.DrawText($"{pokemon.Name} hat {move.Name} erlernt!", DialogBoxContinueMode.User);
        moveSelectionUI.Assign(pokemon);
    }

    public Coroutine TryUseItemOnPokemon(IItem item, IPokemon pokemon, IEnumerator animation, System.Action<bool> success)
        => StartCoroutine(TryUseItemOnPokemonCoroutine(item, pokemon, animation, success));

    IEnumerator TryUseItemOnPokemonCoroutine(IItem item, IPokemon pokemon, IEnumerator animation, System.Action<bool> success)
    {
        // TODO Implement healing volatile status
        if (item.Data.Value.CanBeUsedOnOwnPokemon)
        {
            if (pokemon.IsFainted && !item.Data.Value.Revives)
                yield return dialogBox.DrawText
                    ($"Du kannst {item.Data.Value.Name} nicht auf besiegte Pok?mon anwenden!", closeAfterFinish: true);
            else if (item.Data.Value.HealsHPOnly && pokemon.IsAtFullHP)
                yield return dialogBox.DrawText(
                    $"KP von {pokemon.Name} ist breits voll!", DialogBoxContinueMode.User, closeAfterFinish: true);
            else if (!pokemon.HasNonVolatileStatusEffect && item.Data.Value.HealsStatusEffectsNonVolatileOnly)
                yield return dialogBox.DrawText(
                    $"Das h?tte keinen Effekt, denn {pokemon.Name} hat keine Statusprobleme.", closeAfterFinish: true);
            else if (!pokemon.HasVolatileStatusEffects && item.Data.Value.HealsStatusEffectsVolatileOnly)
                yield return dialogBox.DrawText($"Das h?tte keinen Effekt!", closeAfterFinish: true);
            else if (pokemon.HasNonVolatileStatusEffect &&
                !item.Data.Value.HealsStatusEffectNonVolatile(pokemon.StatusEffectNonVolatile) &&
                item.Data.Value.HealsStatusEffectsNonVolatileOnly)
                yield return dialogBox.DrawText(
                    $"Du kannst {pokemon.StatusEffectNonVolatile.Data.NameSubject} von {pokemon.Name} damit nicht heilen.", closeAfterFinish: true);
            else if (pokemon.HasVolatileStatusEffects &&
                !item.Data.Value.HealsStatusEffectVolatile(pokemon.StatusEffectsVolatile) &&
                item.Data.Value.HealsStatusEffectsNonVolatileOnly)
            {
                print(pokemon.HasVolatileStatusEffects);
                print(!item.Data.Value.HealsStatusEffectVolatile(pokemon.StatusEffectsVolatile));
                print(item.Data.Value.HealsStatusEffectsNonVolatileOnly);
                yield return dialogBox.DrawText($"Das h?tte keinen Effekt!", closeAfterFinish: true);
            }
                
            else
            {
                yield return dialogBox.DrawChoiceBox($"{item.Data.Value.Name} {pokemon.Name} geben?");
                if (dialogBox.GetChosenIndex() == 0)
                {
                    yield return UseItemOnPokemon(item, pokemon, animation);
                    dialogBox.Close();
                    success?.Invoke(true);
                    yield break;
                }
                dialogBox.Close();
            }
        }
        else
            yield return dialogBox.DrawText($"{item.Data.Value.Name} kann nicht auf deine Pokemon angewendet werden!", DialogBoxContinueMode.User, closeAfterFinish: true);
        success?.Invoke(false);
    }

    IEnumerator UseItemOnPokemon(IItem item, IPokemon pokemon, IEnumerator animation)
    {
        print($"Use item {item.Data.Value.Name}");
        if (item.Data.Value.HealsHPFully)
        {
            pokemon.HealHPFully();
            yield return animation;
            yield return dialogBox.DrawText($"Die KP von {pokemon.Name} wurde vollst?ndig aufgef?llt!", closeAfterFinish: true);
        }
        else if(item.Data.Value.HpHealed > 0)
        {
            pokemon.HealHP(item.Data.Value.HpHealed);
            yield return animation;
            yield return dialogBox.DrawText($"Die KP von {pokemon.Name} wurde um {item.Data.Value.HpHealed} Punkte aufgef?llt!", closeAfterFinish: true);
        }

        print(item.Data.Value.HealsStatusEffectNonVolatile(pokemon.StatusEffectNonVolatile));
        if (item.Data.Value.HealsStatusEffectNonVolatile(pokemon.StatusEffectNonVolatile))
        {
            print(pokemon.HasNonVolatileStatusEffect);
            if (pokemon.HasNonVolatileStatusEffect)
            {
                string text = pokemon.StatusEffectNonVolatile.Data.HealText;
                if (item.Data.Value.HealsAllStatusEffectsNonVolatile)
                    pokemon.HealAllStatusEffectsNonVolatile();
                else
                    pokemon.HealStatusEffectNonVolatile(item.Data.Value.NonVolatileStatusHealed.Value);
                yield return dialogBox.DrawText(
                    TextKeyManager.ReplaceKey(TextKeyManager.TextKeyPokemon, text, pokemon.Name), closeAfterFinish: true);
            }
        }

        print(item.Data.Value.HealsStatusEffectVolatile(pokemon.StatusEffectsVolatile));
        if (item.Data.Value.HealsStatusEffectVolatile(pokemon.StatusEffectsVolatile))
        {
            IStatusEffect[] healedStatus;
            if (item.Data.Value.HealsAllStatusEffectsVolatile)
            {
                healedStatus = pokemon.StatusEffectsVolatile.ToArray();
                pokemon.HealAllVolatileStatusEffects();
            }
            else
                healedStatus = new IStatusEffect[] { pokemon.HealStatusEffectVolatile(item.Data.Value.VolatileStatusHealed.Value) };

            foreach (StatusEffect s in healedStatus)
            {
                if (healedStatus is null)
                    continue;
                yield return dialogBox.DrawText(
                    TextKeyManager.ReplaceKey(TextKeyManager.TextKeyPokemon, s.Data.HealText, pokemon.Name), closeAfterFinish: true);
            }
        }

        SfxHandler.Play(healSound);

        if (item.Data.Value.Consumable)
            PlayerData.Instance.TryTakeItem(item);

        yield return null;
    }

    public IEnumerator HandleWalkDamage(System.Action playerHasBeenDefeatedCallback)
    {
        foreach (Pokemon pokemon in PlayerData.Instance.Pokemons)
        {
            if (pokemon.StatusEffectNonVolatile is null)
                continue;
            IStatusEffectData statusEffect = pokemon.StatusEffectNonVolatile.Data;
            pokemon.InflictDamageOverTime();
            if (pokemon.IsFainted)
            {
                yield return dialogBox.DrawText($"{pokemon.Name} wurde durch {statusEffect.NameSubject} besiegt.", closeAfterFinish: true);
                if (PlayerData.Instance.IsDefeated())
                {
                    playerHasBeenDefeatedCallback?.Invoke();
                    Services.Get<IPlayerCharacter>().Defeat();
                }
            }
        }
    }
}
