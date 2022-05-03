using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour, IBattleUI
{
    [SerializeField] private MoveButtonsCollection moveButtons;
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

    private Pokemon playerPokemon => playerData.pokemons[playerPokemonIndex];
    private Pokemon opponentPokemon => opponentData.pokemons[opponentPokemonIndex];

    private void Awake()
    {
        Services.Register(this as IBattleUI);
        gameObject.SetActive(false);
    }

    private void Update()
    {
    }

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
}
