using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonManager : MonoBehaviour
{
    public static PokemonManager Instance;

    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] AudioClip xpFullSound;
    [SerializeField] AudioClip levelGainMusic;
    [SerializeField] AudioClip healSound;

    public PokemonManager() => Instance = this;

    public IEnumerator GrowLevel(Pokemon pokemon, System.Action<bool> uiRefreshCallback)
    {
        SfxHandler.Play(xpFullSound);
        yield return new WaitForSeconds(xpFullSound.length);

        pokemon.GrowLevel();
        uiRefreshCallback(false);

        BgmHandler.Instance.PlayMFX(levelGainMusic);
        yield return DialogBox.Instance.DrawText($"{pokemon.Name} erreicht Level {pokemon.level}!", DialogBoxContinueMode.User, closeAfterFinish: true);

        MoveData move;
        if (pokemon.data.levelToMoveDataMap.TryGetValue(pokemon.level, out move))
            yield return TryLearnMove(pokemon, move);
    }

    public IEnumerator TryLearnMove(Pokemon pokemon, MoveData move)
    {
        if (pokemon.moves.Count >= 4)
        {
            while (true)
            {
                // Should an move be forgotten?
                yield return DialogBox.Instance.DrawText($"{pokemon.Name} will {move.fullName} erlernen.", DialogBoxContinueMode.User);
                yield return DialogBox.Instance.DrawText($"Aber {pokemon.Name} kann keine weiteren Attacken mehr erlernen!", DialogBoxContinueMode.User);
                yield return DialogBox.Instance.DrawChoiceBox($"Soll {pokemon.Name} eine Attacke vergessen um {move.fullName} zu erlernen?");
                if (DialogBox.Instance.GetChosenIndex() == 0)
                {
                    // Forget move

                    // Let player choose move
                    DialogBox.Instance.Close();
                    Move choosenMove = null;
                    bool goBack = false;
                    moveSelectionUI.Open((SelectableUIElement selection, bool back) =>
                    {
                        choosenMove = selection is null ? null : pokemon.moves[selection.GetIndex()];
                        goBack = back;
                    });

                    print("wait for play to choose move");
                    yield return new WaitUntil(() => choosenMove != null || goBack);
                    moveSelectionUI.Close();

                    if (!goBack)
                    {
                        yield return DialogBox.Instance.DrawText($"Und...", DialogBoxContinueMode.User);
                        yield return DialogBox.Instance.DrawText($"Zack!", DialogBoxContinueMode.User);
                        yield return DialogBox.Instance.DrawText($"{pokemon.Name} hat {choosenMove.data.fullName} vergessen, und...", DialogBoxContinueMode.User);
                        pokemon.ReplaceMove(choosenMove, move);
                        break;
                    }
                }
                else
                {
                    // Dont forget move
                    yield return DialogBox.Instance.DrawText($"{move.fullName} wurde nicht erlernt.", DialogBoxContinueMode.User);
                    yield break;
                }
            }
        }
        else
            pokemon.AddMove(move);
        yield return DialogBox.Instance.DrawText($"{pokemon.Name} hat {move.fullName} erlernt!", DialogBoxContinueMode.User);
        moveSelectionUI.Assign(pokemon);
    }

    public Coroutine TryUseItemOnPokemon(Item item, Pokemon pokemon, IEnumerator animation, System.Action<bool> success)
        => StartCoroutine(TryUseItemOnPokemonCoroutine(item, pokemon, animation, success));

    IEnumerator TryUseItemOnPokemonCoroutine(Item item, Pokemon pokemon, IEnumerator animation, System.Action<bool> success)
    {
        // TODO Implement healing volatile status
        if (item.data.CanBeUsedOnOwnPokemon)
        {
            if (pokemon.isFainted && !item.data.revives)
                yield return DialogBox.Instance.DrawText
                    ($"Du kannst {item.data.fullName} nicht auf besiegte Pok?mon anwenden!", closeAfterFinish: true);
            else if (item.data.HealsHPOnly && pokemon.isAtFullHP)
                yield return DialogBox.Instance.DrawText(
                    $"KP von {pokemon.Name} ist breits voll!", DialogBoxContinueMode.User, closeAfterFinish: true);
            else if (!pokemon.hasNonVolatileStatusEffect && item.data.HealsStatusEffectsNonVolatileOnly)
                yield return DialogBox.Instance.DrawText(
                    $"Das h?tte keinen Effekt, denn {pokemon.Name} hat keine Statusprobleme.", closeAfterFinish: true);
            else if (!pokemon.hasVolatileStatusEffects && item.data.HealsStatusEffectsVolatileOnly)
                yield return DialogBox.Instance.DrawText($"Das h?tte keinen Effekt!", closeAfterFinish: true);
            else if (pokemon.hasNonVolatileStatusEffect &&
                !item.data.HealsStatusEffectNonVolatile(pokemon.statusEffectNonVolatile) &&
                item.data.HealsStatusEffectsNonVolatileOnly)
                yield return DialogBox.Instance.DrawText(
                    $"Du kannst {pokemon.statusEffectNonVolatile.data.nameSubject} von {pokemon.Name} damit nicht heilen.", closeAfterFinish: true);
            else if (pokemon.hasVolatileStatusEffects &&
                !item.data.HealsStatusEffectVolatile(pokemon.statusEffectsVolatile) &&
                item.data.HealsStatusEffectsNonVolatileOnly)
            {
                print(pokemon.hasVolatileStatusEffects);
                print(!item.data.HealsStatusEffectVolatile(pokemon.statusEffectsVolatile));
                print(item.data.HealsStatusEffectsNonVolatileOnly);
                yield return DialogBox.Instance.DrawText($"Das h?tte keinen Effekt!", closeAfterFinish: true);
            }
                
            else
            {
                yield return DialogBox.Instance.DrawChoiceBox($"{item.data.fullName} {pokemon.Name} geben?");
                if (DialogBox.Instance.GetChosenIndex() == 0)
                {
                    yield return UseItemOnPokemon(item, pokemon, animation);
                    DialogBox.Instance.Close();
                    success?.Invoke(true);
                    yield break;
                }
                DialogBox.Instance.Close();
            }
        }
        else
            yield return DialogBox.Instance.DrawText($"{item.data.fullName} kann nicht auf deine Pokemon angewendet werden!", DialogBoxContinueMode.User, closeAfterFinish: true);
        success?.Invoke(false);
    }

    IEnumerator UseItemOnPokemon(Item item, Pokemon pokemon, IEnumerator animation)
    {
        print($"Use item {item.data.fullName}");
        if (item.data.healsHPFully)
        {
            pokemon.HealHPFully();
            yield return animation;
            yield return DialogBox.Instance.DrawText($"Die KP von {pokemon.Name} wurde vollst?ndig aufgef?llt!", closeAfterFinish: true);
        }
        else if(item.data.hpHealed > 0)
        {
            pokemon.HealHP(item.data.hpHealed);
            yield return animation;
            yield return DialogBox.Instance.DrawText($"Die KP von {pokemon.Name} wurde um {item.data.hpHealed} Punkte aufgef?llt!", closeAfterFinish: true);
        }

        print(item.data.HealsStatusEffectNonVolatile(pokemon.statusEffectNonVolatile));
        if (item.data.HealsStatusEffectNonVolatile(pokemon.statusEffectNonVolatile))
        {
            print(pokemon.hasNonVolatileStatusEffect);
            if (pokemon.hasNonVolatileStatusEffect)
            {
                string text = pokemon.statusEffectNonVolatile.data.healText;
                if (item.data.healsAllStatusEffectsNonVolatile)
                    pokemon.HealAllStatusEffectsNonVolatile();
                else
                    pokemon.HealStatusEffectNonVolatile(item.data.nonVolatileStatusHealed);
                yield return DialogBox.Instance.DrawText(
                    TextKeyManager.ReplaceKey(TextKeyManager.TextKeyPokemon, text, pokemon.Name), closeAfterFinish: true);
            }
        }

        print(item.data.HealsStatusEffectVolatile(pokemon.statusEffectsVolatile));
        if (item.data.HealsStatusEffectVolatile(pokemon.statusEffectsVolatile))
        {
            StatusEffect[] healedStatus;
            if (item.data.healsAllStatusEffectsVolatile)
            {
                healedStatus = pokemon.statusEffectsVolatile.ToArray();
                pokemon.HealAllVolatileStatusEffects();
            }
            else
                healedStatus = new StatusEffect[] { pokemon.HealStatusEffectVolatile(item.data.volatileStatusHealed) };

            foreach (StatusEffect s in healedStatus)
            {
                if (healedStatus is null)
                    continue;
                yield return DialogBox.Instance.DrawText(
                    TextKeyManager.ReplaceKey(TextKeyManager.TextKeyPokemon, s.data.healText, pokemon.Name), closeAfterFinish: true);
            }
        }

        SfxHandler.Play(healSound);

        if (item.data.consumable)
            PlayerData.Instance.TryTakeItem(item);

        yield return null;
    }

    public IEnumerator HandleWalkDamage(System.Action playerHasBeenDefeatedCallback)
    {
        foreach (Pokemon pokemon in PlayerData.Instance.pokemons)
        {
            if (pokemon.statusEffectNonVolatile is null)
                continue;
            StatusEffectData statusEffect = pokemon.statusEffectNonVolatile.data;
            pokemon.InflictDamageOverTime();
            if (pokemon.isFainted)
            {
                yield return DialogBox.Instance.DrawText($"{pokemon.Name} wurde durch {statusEffect.nameSubject} besiegt.", closeAfterFinish: true);
                if (PlayerData.Instance.IsDefeated())
                {
                    playerHasBeenDefeatedCallback?.Invoke();
                    PlayerCharacter.Instance.Defeat();
                }
            }
        }
    }
}