using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour, IBattleUI
{
    readonly private static int PLAYER = 0;
    readonly private static int OPPONENT = 1;

    [SerializeField] private MoveSelectionUI moveButtons;
    [SerializeField] private Image playerPokemonImage;
    [SerializeField] private Image opponentImage;
    [SerializeField] private Image opponentPokemonImage;
    [SerializeField] private PlayerPokemonStatsUI playerStats;
    [SerializeField] private PokemonStatsUI opponentStats;

    private enum BattleState
    {
        None,
        ChoosingMove,
        BatlleMenu,
    }

    private BattleState state;
    private CharacterData playerData;
    private NPCData opponentData;
    private int playerPokemonIndex = 0;
    private int opponentPokemonIndex = 0;

    private Pokemon playerPokemon => GeActivePokemon(playerData);
    private Pokemon opponentPokemon => GeActivePokemon(opponentData);

    private PokemonStatsUI[] stats;
    private CharacterData[] characterData;

    private void Awake()
    {
        Services.Register(this as IBattleUI);
        gameObject.SetActive(false);
        stats = new PokemonStatsUI[] { playerStats, opponentStats };
    }

    private void Update()
    {
    }

    private Pokemon GeActivePokemon(CharacterData character) => character.pokemons[playerPokemonIndex];

    public void Initialize(CharacterData playerData, NPCData opponentData)
    {
        this.playerData = playerData;
        this.opponentData = opponentData;

        moveButtons.AssignMoves(playerPokemon.moves.ToArray());
        playerPokemonImage.sprite = playerPokemon.data.backSprite;
        opponentPokemonImage.sprite = opponentPokemon.data.frontSprite;
        opponentImage.sprite = opponentData.sprite;

        playerStats.AssignPokemon(playerPokemon);
        opponentStats.AssignPokemon(opponentPokemon);

        gameObject.SetActive(true);

        state = BattleState.ChoosingMove;
    }

    public void DoPlayerMove(Move move)
    {
        DoMove(PLAYER, OPPONENT, move);
    }

    private void DoMove(int attacker, int receiver, Move move)
    {
        CharacterData attackerCharacter = characterData[attacker];
        CharacterData receiverCharacter = characterData[receiver];

        Pokemon receiverPokemon = GeActivePokemon(receiverCharacter);

        bool critical;
        Effectiveness effectiveness;
        int damage = move.GetDamageAgainst(receiverPokemon, out critical, out effectiveness);

        receiverPokemon.InflictDamage(damage);

        stats[receiver].Refresh();
    }
}
