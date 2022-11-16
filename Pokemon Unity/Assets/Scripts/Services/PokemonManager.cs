using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonManager : ManagerWithMoveSelection, IPokemonManager
{
    public delegate void UserMoveChooseEventHandler(Move move, bool goBack);
    public event UserMoveChooseEventHandler UserChooseMoveEvent;

    public PokemonManager() => Services.Register(this as IPokemonManager);

    private void Awake() => Initialize();

    public void ChoosePlayerMove(Move move, bool goBack) => UserChooseMoveEvent?.Invoke(move, goBack);

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
                    moveSelectionUI.Open();
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
                    moveSelectionUI.Close();
                    UserChooseMoveEvent -= action;

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
    }
}
