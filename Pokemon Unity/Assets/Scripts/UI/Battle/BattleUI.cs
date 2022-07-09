using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour, IBattleUI
{
    [SerializeField] private MoveSelectionUI moveSelection;
    [SerializeField] private PokemonSprite playerPokemonSprite;
    [SerializeField] private PokemonSprite opponentPokemonSprite;
    [SerializeField] private TrainerSprite opponentSprite;
    [SerializeField] private PlayerPokemonStatsUI playerStats;
    [SerializeField] private PokemonStatsUI opponentStats;
    [SerializeField] private float hpRefreshSpeed = 1f;

    private PokemonStatsUI[] stats;
    private PokemonSprite[] pokemonSprites;

    public BattleUI() => Services.Register(this as IBattleUI);

    private void Awake()
    {
        stats = new PokemonStatsUI[] { playerStats, opponentStats };
        pokemonSprites = new PokemonSprite[] { playerPokemonSprite, opponentPokemonSprite };
    }

    public void Initialize(CharacterData playerData, NPCData opponentData, Pokemon playerPokemon, Pokemon opponentPokemon)
    {
        moveSelection.AssignMoves(playerPokemon.moves.ToArray());
        playerPokemonSprite.SetSprite(playerPokemon.data.backSprite);
        opponentPokemonSprite.SetSprite(opponentPokemon.data.frontSprite);
        opponentSprite.SetSprite(opponentData.sprite);

        playerStats.AssignPokemon(playerPokemon);
        opponentStats.AssignPokemon(opponentPokemon);

        EventManager.Pause();
        gameObject.SetActive(true);
    }

    public void Close() => gameObject.SetActive(false);
        
    public void RefreshHP(int character) => stats[character].RefreshHP();

    public System.Func<bool> RefreshHPAnimated(int character) => stats[character].RefreshHPAnimated(hpRefreshSpeed);

    public System.Func<bool> PlayMoveAnimation(int attacker, Move move)
    {
        PokemonSprite sprite = pokemonSprites[attacker];
        sprite.PlayAnimation(move);
        return sprite.IsPlayingAnimation;
    }

    public System.Func<bool> MakeOpponentAppear()
    {
        opponentSprite.PlayAppearAnimation();
        return opponentSprite.IsPlayingAnimation;
    }

    public System.Func<bool> MakeOpponentDisappear()
    {
        opponentSprite.PlayDisappearAnimation();
        return opponentSprite.IsPlayingAnimation;
    }

    public System.Func<bool> PlayFaintAnimation(int faintedOwner)
    {
        PokemonSprite sprite = pokemonSprites[faintedOwner];
        sprite.PlayFaintAnimation();
        return sprite.IsPlayingAnimation;
    }

    public void SetMoveSelectionActive(bool active) => moveSelection.gameObject.SetActive(active);
}
