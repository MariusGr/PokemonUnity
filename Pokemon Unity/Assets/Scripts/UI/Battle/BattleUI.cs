using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : InputConsumer, IBattleUI
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private BattleMenu battleMenu;
    [SerializeField] private PartySelection partySelection;
    [SerializeField] private BagUI bagSelection;
    [SerializeField] private PokemonSprite playerPokemonSprite;
    [SerializeField] private PokemonSprite opponentPokemonSprite;
    [SerializeField] private TrainerSprite opponentSprite;
    [SerializeField] private PlayerPokemonStatsBattleUI playerStats;
    [SerializeField] private PokemonStatsUI opponentStats;
    [SerializeField] private MoveSelectionUI moveSelectionUI;
    [SerializeField] private PokeBallAnimation ballAnimation;
    [SerializeField] private FadeBlack fadeBlack;
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

    public Coroutine Open(ICharacterData playerData, IPokemon playerPokemon, IPokemon opponentPokemon)
    {
        canvas.enabled = false;
        Open();
        HidePokeBallAnimation();
        ballAnimation.ResetAnimation();
        ShowOpponent();
        Initialize(playerData, playerPokemon, opponentPokemon);
        return StartCoroutine(OpenAnimation());
    }

    public new Coroutine Close()
        => StartCoroutine(CloseAnimation());

    IEnumerator OpenAnimation()
    {
        yield return fadeBlack.FadeToBlack();
        canvas.enabled = true;
        yield return fadeBlack.FadeFromBlack();
    }

    IEnumerator CloseAnimation()
    {
        yield return fadeBlack.FadeToBlack();
        canvas.enabled = false;
        yield return fadeBlack.FadeFromBlack();
        base.Close();
    }

    private void Initialize(ICharacterData playerData, IPokemon playerPokemon, IPokemon opponentPokemon)
    {
        SwitchToPokemon(Constants.PlayerIndex, playerPokemon);
        SwitchToPokemon(Constants.OpponentIndex, opponentPokemon);
        opponentSprite.SetVisiblity(false);
        partySelection.AssignElements(playerData.Pokemons.ToArray());
    }

    public void SwitchToPokemon(int characterIndex, IPokemon pokemon)
    {
        stats[characterIndex].AssignElement(pokemon);
        pokemonSprites[characterIndex].SetSprite(pokemon.Data.GetBattleSprite(characterIndex));
        if (characterIndex == Constants.PlayerIndex)
            moveSelectionUI.Assign(pokemon);
    }

    public void Refresh(int character) => stats[character].Refresh();
    public void RefreshPlayerStats(bool refreshXP = false) => playerStats.Refresh(refreshXP);
    public void RefreshHP(int character) => stats[character].RefreshHP();
    public void RefreshXP() => playerStats.RefreshXP();
    public void ResetXP() => playerStats.ResetXP();
    public void RefreshMove(IMove move) => moveSelectionUI.RefreshMove(move);

    public IEnumerator RefreshHPAnimated(int character)
    {
        yield return stats[character].RefreshHPAnimated(hpRefreshSpeed);
    }
    public IEnumerator RefreshXPAnimated() => playerStats.RefreshXPAnimated(xpRefreshSpeed);
    public IEnumerator PlayMoveAnimation(int attacker, IMove move) => pokemonSprites[attacker].PlayMoveAnimation(move);
    public IEnumerator PlayBlinkAnimation(int blinkingPokemon) => pokemonSprites[blinkingPokemon].PlayBlinkAnimation();
    public IEnumerator PlayFaintAnimation(int faintedOwner) => pokemonSprites[faintedOwner].PlayFaintAnimation();
    public IEnumerator PlayInflictStatusAnimation(int owner) => pokemonSprites[owner].PlayInflictStatusAnimation();
    public IEnumerator PlayStatUpAnimation(int owner) => pokemonSprites[owner].PlayStatUpAnimation();
    public IEnumerator PlayStatDownAnimation(int owner) => pokemonSprites[owner].PlayStatDownAnimation();

    public IEnumerator PlayThrowAnimation()
    {
        ballAnimation.gameObject.SetActive(true);
        return ballAnimation.PlayThrowAnimation();
    }

    public IEnumerator PlayShakeAnimation()
    {
        ballAnimation.gameObject.SetActive(true);
        return ballAnimation.PlayShakeAnimation();
    }

    public void HideOpponent() => opponentPokemonSprite.SetSpriteVisible(false);
    public void ShowOpponent() => opponentPokemonSprite.SetSpriteVisible(true);
    public void HidePokeBallAnimation() => ballAnimation.gameObject.SetActive(false);

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
        => bagSelection.OpenBattle(callback);
    public void OpenMoveSelection(System.Action<ISelectableUIElement, bool> callback, IPokemon pokemon)
    {
        moveSelectionUI.AssignElements(pokemon.Moves.ToArray());
        moveSelectionUI.Open(callback);
    }

    public void CloseBattleMenu() => battleMenu.Close();
    public void ClosePokemonSwitchSelection() => partySelection.Close();
    public void CloseMoveSelection() => moveSelectionUI.Close();
    public void CloseBagSelection() => bagSelection.Close();

    public override bool ProcessInput(InputData input) => false;
}
