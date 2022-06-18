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
    }

    private BattleState state;
    private CharacterData playerData;
    private NPCData opponentData;
    private int activePlayer = 0;
    private int otherPlayer = 1;
    private int playerPokemonIndex = 0;
    private int opponentPokemonIndex = 0;

    public delegate void UserMoveChooseEventHandler(Move move);
    public event UserMoveChooseEventHandler UserChooseMoveEvent;

    private CharacterData[] characterData;

    private Pokemon playerPokemon => GeActivePokemon(playerData);
    private Pokemon opponentPokemon => GeActivePokemon(opponentData);

    private IBattleUI ui;
    private IDialogBox dialogBox;

    public BattleManager() => Services.Register(this as IBattleManager);

    void Awake()
    {
        ui = Services.Get<IBattleUI>();
        dialogBox = Services.Get<IDialogBox>();
    }

    private Pokemon GeActivePokemon(CharacterData character) => character.pokemons[playerPokemonIndex];

    public void StartNewBattle(CharacterData playerData, NPCData opponentData)
    {
        this.playerData = playerData;
        this.opponentData = opponentData;
        characterData = new CharacterData[] { this.playerData, this.opponentData };

        state = BattleState.ChoosingMove;
        playerPokemonIndex = 0;
        opponentPokemonIndex = 0;
        activePlayer = 0;

        ui.Initialize(this.playerData, this.opponentData, playerPokemon, opponentPokemon);

        StartCoroutine(RoundCoroutine());
    }

    private IEnumerator RoundCoroutine()
    {
        activePlayer = 0;
        Move opponentMove = GetOpponentMove();
        Move playerMove = null;
        yield return GetPlayerMove(choosenMove => playerMove = choosenMove);

        IEnumerator playerCoroutine = MoveCoroutine(Constants.PlayerIndex, Constants.OpponentIndex, playerMove);
        IEnumerator opponentCoroutine = MoveCoroutine(Constants.OpponentIndex, Constants.PlayerIndex, opponentMove);

        if (playerMove.IsFaster(opponentMove))
        {
            yield return playerCoroutine;
            yield return opponentCoroutine;
        }
        else
        {
            yield return opponentCoroutine;
            yield return playerCoroutine;
        }
    }

    private Move GetOpponentMove()
    {
        // TODO: implement more intelligent move choose
        return opponentPokemon.moves[UnityEngine.Random.Range(0, opponentPokemon.moves.Count)];
    }

    private IEnumerator GetPlayerMove(Action<Move> callback)
    {
        ui.SetMoveSelectionActive(true);

        Move choosenMove = null;
        UserMoveChooseEventHandler action = (Move move) => choosenMove = move;
        UserChooseMoveEvent += action;
        yield return new WaitUntil(() => choosenMove != null);
        UserChooseMoveEvent -= action;
        callback(choosenMove);
    }

    public void ChoosePlayerMove(Move move) => UserChooseMoveEvent?.Invoke(move);

    private IEnumerator MoveCoroutine(int attacker, int target, Move move)
    {
        CharacterData attackerCharacter = characterData[attacker];
        CharacterData targetCharacter = characterData[target];

        Pokemon attackerPokemon = GeActivePokemon(attackerCharacter);
        Pokemon targetPokemon = GeActivePokemon(targetCharacter);
        
        // Play attack animation
        print(Services.Get<IDialogBox>());
        yield return dialogBox.DrawText(new string[] { $"{attackerPokemon.Name} setzt {move.data.fullName} ein!" }, DialogBoxCloseMode.External);
        yield return new WaitForSeconds(1f);
        yield return new WaitWhile(ui.PlayMoveAnimation(attacker, move));
        yield return new WaitForSeconds(1f);
        
        // Deal Damage to target
        bool critical;
        Effectiveness effectiveness;
        int damage = move.GetDamageAgainst(attackerPokemon, targetPokemon, out critical, out effectiveness);

        targetPokemon.InflictDamage(damage);

        yield return new WaitWhile(ui.RefreshHPAnimated(target));

        if (effectiveness != Effectiveness.Normal)
            yield return dialogBox.DrawText(effectiveness, DialogBoxCloseMode.User);

        if (critical)
            yield return dialogBox.DrawText(new string[] { $"Es war ein kritischer Treffer!" }, DialogBoxCloseMode.User);

        dialogBox.Close();
    }
}
