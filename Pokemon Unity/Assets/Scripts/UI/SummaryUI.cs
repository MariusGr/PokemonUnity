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
        pokemonImage.sprite = pokemon.data.frontSprite;

        Type1Image.sprite = pokemon.data.pokemonTypes[0].titleSprite;
        Sprite type2Sprite = pokemon.data.GetType2Sprite();
        Type2Image.sprite = type2Sprite;
        if (type2Sprite is null)
            Type2Image.gameObject.SetActive(false);
        else
            Type2Image.gameObject.SetActive(true);

        dexNoText.text = pokemon.data.dex.ToString();
        speciesText.text = pokemon.data.fullName.ToString().ToUpper();
        idText.text = pokemon.id.ToString();
        metDateText.text = pokemon.metDate.ToString();
        metMapText.text = pokemon.metMap.ToString();
        metLevelText.text = pokemon.metLevel.ToString();
        statsText.text = $"{pokemon.attack}\n{pokemon.defense}\n{pokemon.specialAttack}\n{pokemon.specialDefense}\n{pokemon.speed}";
        moveSelection.Assign(pokemon);
        moveSelection.AssignOnSelectCallback(RefreshMoveSelection);
        CloseMoveSelection();
    }

    public void RefreshMoves(Pokemon pokemon)
    {
        moveSelection.Assign(pokemon);
        RefreshMoveSelection(moveSelection.selectedElement.payload);
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
        moveDescriptionText.text = move.data.description;
        moveAccuracyText.text = move.data.accuracy.ToString();
        movePowerText.text = move.data.power.ToString();
        moveCategoryImage.sprite = move.data.category.icon;
    }

    public override void RefreshHP()
    {
        base.RefreshHP();
        hp.text = $"{pokemon.hp}/{pokemon.maxHp}";
    }

    public override void RefreshXP()
    {
        xpNextLevelText.text = pokemon.xpNeededForNextLevel.ToString();
        xpText.text = pokemon.xp.ToString();
        base.RefreshXP();
    }
}
