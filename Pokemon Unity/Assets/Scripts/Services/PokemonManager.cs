using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonManager : ManagerWithDialogBox, IPokemonManager
{
    public static PokemonManager Instance;

    [SerializeField] IMoveSelectionUI moveSelectionUI;

    public PokemonManager()
    {
        Instance = this;
        Services.Register(this as IPokemonManager);
    }

    private void Awake() => Initialize();

    public IEnumerator GrowLevel(Pokemon pokemon, System.Action<bool> uiRefreshCallback)
    {
        pokemon.GrowLevel();
        uiRefreshCallback(false);
        yield return dialogBox.DrawText($"{pokemon.Name} erreicht Level {pokemon.level}!", DialogBoxContinueMode.User, closeAfterFinish: true);

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
                yield return dialogBox.DrawText($"{pokemon.Name} will {move.fullName} erlernen.", DialogBoxContinueMode.User);
                yield return dialogBox.DrawText($"Aber {pokemon.Name} kann keine weiteren Attacken mehr erlernen!", DialogBoxContinueMode.User);
                yield return dialogBox.DrawChoiceBox($"Soll {pokemon.Name} eine Attacke vergessen um {move.fullName} zu erlernen?");
                if (dialogBox.GetChosenIndex() == 0)
                {
                    // Forget move

                    // Let player choose move
                    dialogBox.Close();
                    Move choosenMove = null;
                    bool goBack = false;
                    moveSelectionUI.Open((ISelectableUIElement selection, bool back) =>
                    {
                        choosenMove = selection is null ? null : pokemon.moves[selection.GetIndex()];
                        goBack = back;
                    });

                    print("wait for play to choose move");
                    yield return new WaitUntil(() => choosenMove != null || goBack);
                    moveSelectionUI.Close();

                    if (!goBack)
                    {
                        yield return dialogBox.DrawText($"Und...", DialogBoxContinueMode.User);
                        yield return dialogBox.DrawText($"Zack!", DialogBoxContinueMode.User);
                        yield return dialogBox.DrawText($"{pokemon.Name} hat {choosenMove.data.fullName} vergessen, und...", DialogBoxContinueMode.User);
                        pokemon.ReplaceMove(choosenMove, move);
                        break;
                    }
                }
                else
                {
                    // Dont forget move
                    yield return dialogBox.DrawText($"{move.fullName} wurde nicht erlernt.", DialogBoxContinueMode.User);
                    yield break;
                }
            }
        }
        else
            pokemon.AddMove(move);
        yield return dialogBox.DrawText($"{pokemon.Name} hat {move.fullName} erlernt!", DialogBoxContinueMode.User);
        moveSelectionUI.Assign(pokemon);
    }

    public Coroutine TryUseItemOnPokemon(Item item, Pokemon pokemon, IEnumerator animation)
        => StartCoroutine(TryUseItemOnPokemonCoroutine(item, pokemon, animation));

    IEnumerator TryUseItemOnPokemonCoroutine(Item item, Pokemon pokemon, IEnumerator animation)
    {
        if (item.data.canBeUsedOnOwnPokemon)
        {
            yield return dialogBox.DrawChoiceBox($"{item.data.fullName} {pokemon.Name} geben?");
            if (dialogBox.GetChosenIndex() == 0)
                yield return UseItemOnPokemon(item, pokemon, animation);
        }
        else
        {
            yield return dialogBox.DrawText($"{item.data.fullName} kann nicht auf deine Pokemon angewendet werden!", DialogBoxContinueMode.User, closeAfterFinish: true);
        }
    }

    IEnumerator UseItemOnPokemon(Item item, Pokemon pokemon, IEnumerator animation)
    {
        if (item.data.hpHealed > 0)
        {
            pokemon.HealHP(item.data.hpHealed);
            yield return animation;
            yield return dialogBox.DrawText($"Die KP von {pokemon.Name} wurde um {item.data.hpHealed} Punkte aufgefüllt!", DialogBoxContinueMode.User, closeAfterFinish: true);
        }
    }
}
