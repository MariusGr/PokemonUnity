using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : InputConsumer, IBattleUI
{
    [SerializeField] private BattleMenu battleMenu;
    [SerializeField] private PartySelection partySelection;
    [SerializeField] private BagUI bagSelection;
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
        partySelection.AssignElements(playerData.pokemons);
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
    public void RefreshMove(Move move) => moveSelectionUI.RefreshMove(move);

    public IEnumerator RefreshHPAnimated(int character) => stats[character].RefreshHPAnimated(hpRefreshSpeed);
    public IEnumerator RefreshXPAnimated() => playerStats.RefreshXPAnimated(xpRefreshSpeed);
    public IEnumerator PlayMoveAnimation(int attacker, Move move) => pokemonSprites[attacker].PlayAnimation(move);
    public IEnumerator PlayBlinkAnimation(int blinkingPokemon) => pokemonSprites[blinkingPokemon].PlayBlinkAnimation();
    public IEnumerator PlayFaintAnimation(int faintedOwner) => pokemonSprites[faintedOwner].PlayFaintAnimation();

    public IEnumerator MakeOpponentAppear()
    {
        opponentSprite.SetVisiblity(true);
        yield return opponentSprite.PlayAppearAnimation();
    }

    public IEnumerator MakeOpponentDisappear()
    {
        opponentSprite.SetVisiblity(true);
        yield return opponentSprite.PlayDisappearAnimation();
    }

    public void OpenBattleMenu(System.Action<BattleOption, bool> callback)
        => battleMenu.Open((ISelectableUIElement selection, bool goBack)
            => callback(selection is null ? BattleOption.None : ((BattleMenuButton)selection).option, false));
    public void OpenPokemonSwitchSelection(System.Action<ISelectableUIElement, bool> callback, bool forceSelection)
        => partySelection.Open(callback, forceSelection: forceSelection, startSelection: PlayerData.Instance.GetFirstAlivePokemonIndex(), battle: true);
    public void OpenBagSelection(System.Action<ISelectableUIElement, bool> callback)
        => bagSelection.OpenBatlle(callback);
    public void OpenMoveSelection(System.Action<ISelectableUIElement, bool> callback, Pokemon pokemon)
    {
        moveSelectionUI.AssignElements(pokemon.moves.ToArray());
        moveSelectionUI.Open(callback);
    }

    public void CloseBattleMenu() => battleMenu.Close();
    public void ClosePokemonSwitchSelection() => partySelection.Close();
    public void CloseMoveSelection() => moveSelectionUI.Close();
    public void CloseBagSelection() => bagSelection.Close();

    public override bool ProcessInput(InputData input) => false;
}
