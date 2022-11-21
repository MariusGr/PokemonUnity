using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : OpenedInputConsumer, IBattleUI
{
    [SerializeField] private BattleMenu battleMenu;
    [SerializeField] private PokemonSwitchSelection pokemonSwitchSelection;
    [SerializeField] private PokemonSprite playerPokemonSprite;
    [SerializeField] private PokemonSprite opponentPokemonSprite;
    [SerializeField] private TrainerSprite opponentSprite;
    [SerializeField] private PlayerPokemonStatsBattleUI playerStats;
    [SerializeField] private PokemonStatsUI opponentStats;
    [SerializeField] private MoveSelectionUI moveSelectionUI;
    [SerializeField] private float hpRefreshSpeed = 1f;
    [SerializeField] private float xpRefreshSpeed = 1f;

    private PokemonStatsUI[] stats;
    private PokemonSprite[] pokemonSprites;

    public BattleUI() => Services.Register(this as IBattleUI);

    private void Awake()
    {
        stats = new PokemonStatsUI[] { playerStats, opponentStats };
        pokemonSprites = new PokemonSprite[] { playerPokemonSprite, opponentPokemonSprite };
    }

    public void Open(CharacterData playerData, Pokemon playerPokemon, Pokemon opponentPokemon)
    {
        Open();
        Initialize(playerData, playerPokemon, opponentPokemon);
    }

    private void Initialize(CharacterData playerData, Pokemon playerPokemon, Pokemon opponentPokemon)
    {
        SwitchToPokemon(Constants.PlayerIndex, playerPokemon);
        SwitchToPokemon(Constants.OpponentIndex, opponentPokemon);
        opponentSprite.SetVisiblity(false);
        pokemonSwitchSelection.AssignElements(playerData.pokemons);
    }

    //TODO needed?
    private void Initialize(CharacterData playerData, NPCData opponentData, Pokemon playerPokemon, Pokemon opponentPokemon)
    {
        opponentSprite.SetSprite(opponentData.sprite);
        opponentStats.AssignPokemon(opponentPokemon);
        Initialize(playerData, playerPokemon, opponentPokemon);
    }

    public void SwitchToPokemon(int characterIndex, Pokemon pokemon)
    {
        stats[characterIndex].AssignPokemon(pokemon);
        pokemonSprites[characterIndex].SetSprite(pokemon.data.GetBattleSprite(characterIndex));
        if (characterIndex == Constants.PlayerIndex)
            moveSelectionUI.Assign(pokemon);
    }

    public void Refresh(int character) => stats[character].Refresh();
    public void RefreshPlayerStats(bool refreshXP = false) => playerStats.Refresh(refreshXP);
    public void RefreshHP(int character) => stats[character].RefreshHP();
    public void RefreshXP() => playerStats.RefreshXP();
    public void ResetXP() => playerStats.ResetXP();
    public System.Func<bool> RefreshHPAnimated(int character) => stats[character].RefreshHPAnimated(hpRefreshSpeed);
    public System.Func<bool> RefreshXPAnimated() => playerStats.RefreshXPAnimated(xpRefreshSpeed);
    public void RefreshMove(Move move) => moveSelectionUI.RefreshMove(move);

    public System.Func<bool> PlayMoveAnimation(int attacker, Move move)
    {
        PokemonSprite sprite = pokemonSprites[attacker];
        sprite.PlayAnimation(move);
        return sprite.IsPlayingAnimation;
    }

    public System.Func<bool> PlayBlinkAnimation(int blinkingPokemon)
    {
        PokemonSprite sprite = pokemonSprites[blinkingPokemon];
        sprite.PlayBlinkAnimation();
        return sprite.IsPlayingAnimation;
    }

    public System.Func<bool> MakeOpponentAppear()
    {
        opponentSprite.SetVisiblity(true);
        opponentSprite.PlayAppearAnimation();
        return opponentSprite.IsPlayingAnimation;
    }

    public System.Func<bool> MakeOpponentDisappear()
    {
        opponentSprite.SetVisiblity(true);
        opponentSprite.PlayDisappearAnimation();
        return opponentSprite.IsPlayingAnimation;
    }

    public System.Func<bool> PlayFaintAnimation(int faintedOwner)
    {
        PokemonSprite sprite = pokemonSprites[faintedOwner];
        sprite.PlayFaintAnimation();
        return sprite.IsPlayingAnimation;
    }

    public void OpenBattleMenu(System.Action<BattleOption, bool> callback)
        => battleMenu.Open((ISelectableUIElement selection, bool goBack)
            => callback(((BattleMenuButton)selection).option, false));
    public void OpenPokemonSwitchSelection(System.Action<ISelectableUIElement, bool> callback, bool forceSelection)
        => pokemonSwitchSelection.Open(callback, forceSelection, PlayerData.Instance.GetFirstAlivePokemonIndex());
    public void OpenMoveSelection(System.Action<ISelectableUIElement, bool> callback) =>
        moveSelectionUI.Open(callback);

    public void CloseBattleMenu() => battleMenu.Close();
    public void ClosePokemonSwitchSelection() => pokemonSwitchSelection.Close();
    public void CloseMoveSelection() => moveSelectionUI.Close();

    public override bool ProcessInput(InputData input) => false;
}
