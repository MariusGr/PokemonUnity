using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour, IBattleUI
{
    [SerializeField] private MoveSelectionUI moveSelection;
    [SerializeField] private Image playerPokemonImage;
    [SerializeField] private Image opponentImage;
    [SerializeField] private Image opponentPokemonImage;
    [SerializeField] private PlayerPokemonStatsUI playerStats;
    [SerializeField] private PokemonStatsUI opponentStats;
    [SerializeField] private PokemonSprite playerPokemonSprite;
    [SerializeField] private PokemonSprite opponentPokemonSprite;
    [SerializeField] private float hpRefreshSpeed = 1f;

    private PokemonStatsUI[] stats;
    private PokemonSprite[] pokemonSprites;

    public BattleUI() => Services.Register(this as IBattleUI);

    private void Awake()
    {
        gameObject.SetActive(false);
        stats = new PokemonStatsUI[] { playerStats, opponentStats };
        pokemonSprites = new PokemonSprite[] { playerPokemonSprite, opponentPokemonSprite };
    }

    public void Initialize(CharacterData playerData, NPCData opponentData, Pokemon playerPokemon, Pokemon opponentPokemon)
    {
        moveSelection.AssignMoves(playerPokemon.moves.ToArray());
        playerPokemonImage.sprite = playerPokemon.data.backSprite;
        opponentPokemonImage.sprite = opponentPokemon.data.frontSprite;
        opponentImage.sprite = opponentData.sprite;

        playerStats.AssignPokemon(playerPokemon);
        opponentStats.AssignPokemon(opponentPokemon);

        gameObject.SetActive(true);
    }
        
    public void RefreshHP(int character) => stats[character].RefreshHP();

    public System.Func<bool> RefreshHPAnimated(int character)
    {
        return stats[character].RefreshHPAnimated(hpRefreshSpeed);
    }

    public System.Func<bool> PlayMoveAnimation(int attacker, Move move)
    {
        PokemonSprite sprite = pokemonSprites[attacker];
        sprite.PlayAnimation(move.data.GetAnimationClip(attacker));
        return sprite.IsPlayingAnimation;
    }

    public void SetMoveSelectionActive(bool active) => moveSelection.gameObject.SetActive(active);
}
