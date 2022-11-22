using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryUI : PlayerPokemonStatsBattleUI, IUIView
{
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
    [SerializeField] MoveSelectionUI moveSelection;
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
        dexNoText.text = pokemon.data.dex.ToString();
        speciesText.text = pokemon.data.fullName.ToString().ToUpper();
        idText.text = pokemon.id.ToString();
        metDateText.text = pokemon.metDate.ToString();
        metMapText.text = pokemon.metMap.ToString();
        metLevelText.text = pokemon.metLevel.ToString();
        statsText.text = $"{pokemon.attack}\n{pokemon.defense}\n{pokemon.specialAttack}\n{pokemon.specialDefense}\n{pokemon.speed}";
        moveSelection.Assign(pokemon);
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
