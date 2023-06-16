using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryUI : PlayerPokemonStatsBattleUI, IUIView
{
    [SerializeField] Image pokemonImage;
    [SerializeField] ShadowedText dexNoText;
    [SerializeField] ShadowedText speciesText;
    [SerializeField] Image Type1Image;
    [SerializeField] Image Type2Image;
    [SerializeField] ShadowedText idText;
    [SerializeField] ShadowedText xpText;
    [SerializeField] ShadowedText xpNextLevelText;
    [SerializeField] ShadowedText metDateText;
    [SerializeField] ShadowedText metMapText;
    [SerializeField] ShadowedText metLevelText;
    [SerializeField] ShadowedText statsText;
    [SerializeField] MoveSelectionSummaryUI moveSelection;
    [SerializeField] Image moveCategoryImage;
    [SerializeField] ShadowedText movePowerText;
    [SerializeField] ShadowedText moveAccuracyText;
    [SerializeField] ShadowedText moveDescriptionText;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    public override void Refresh()
    {
        base.Refresh();
        RefreshXP();
        pokemonImage.sprite = pokemon.Data.FrontSprite;

        Type1Image.sprite = pokemon.Data.PokemonTypes[0].TitleSprite;
        Sprite type2Sprite = pokemon.Data.GetType2Sprite();
        Type2Image.sprite = type2Sprite;
        if (type2Sprite is null)
            Type2Image.gameObject.SetActive(false);
        else
            Type2Image.gameObject.SetActive(true);

        dexNoText.text = pokemon.Data.Dex.ToString();
        speciesText.text = pokemon.Data.FullName.ToString().ToUpper();
        idText.text = pokemon.Id.ToString();
        metDateText.text = pokemon.MetDate.ToString();
        metMapText.text = pokemon.MetMap.ToString();
        metLevelText.text = pokemon.MetLevel.ToString();
        statsText.text = $"{pokemon.AttackUnmodified}\n{pokemon.DefenseUnmodified}\n{pokemon.SpecialAttackUnmodified}\n{pokemon.SpecialDefenseUnmodified}\n{pokemon.SpeedUnmodified}";
        moveSelection.Assign(pokemon);
        moveSelection.AssignOnSelectCallback(RefreshMoveSelection);
        CloseMoveSelection();
    }

    public void RefreshMoves(IPokemon pokemon)
    {
        moveSelection.Assign(pokemon);
        RefreshMoveSelection(moveSelection.selectedElement.GetPayload());
    }

    public void OpenMoveSelection(System.Action<ISelectableUIElement, bool> callback) => moveSelection.Open(callback);

    public void CloseMoveSelection()
    {
        moveSelection.Close();
        moveSelection.Show();
        moveDescriptionText.gameObject.SetActive(false);
        moveAccuracyText.gameObject.SetActive(false);
        movePowerText.gameObject.SetActive(false);
        moveCategoryImage.gameObject.SetActive(false);
    }

    public void RefreshMoveSelection(object selection)
    {
        Move move = (Move)selection;
        moveDescriptionText.gameObject.SetActive(true);
        moveAccuracyText.gameObject.SetActive(true);
        movePowerText.gameObject.SetActive(true);
        moveCategoryImage.gameObject.SetActive(true);
        moveDescriptionText.text = move.Data.Description;
        moveAccuracyText.text = move.Data.Accuracy.ToString();
        movePowerText.text = move.Data.Power.ToString();
        moveCategoryImage.sprite = move.Data.Category.Icon;
    }

    public override void RefreshHP()
    {
        base.RefreshHP();
        hp.text = $"{pokemon.hp}/{pokemon.MaxHp}";
    }

    public override void RefreshXP()
    {
        xpNextLevelText.text = pokemon.xpNeededForNextLevel.ToString();
        xpText.text = pokemon.xp.ToString();
        base.RefreshXP();
    }
}
