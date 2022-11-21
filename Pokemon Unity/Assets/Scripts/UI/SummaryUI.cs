using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryUI : PlayerPokemonStatsBattleUI, IUIView
{
    [SerializeField] Text dexNoText;
    [SerializeField] Text speciesText;
    [SerializeField] Image Type1Image;
    [SerializeField] Image Type2Image;
    [SerializeField] Text idText;
    [SerializeField] Text xpText;
    [SerializeField] Text xpNextLevelText;
    [SerializeField] Text metDateText;
    [SerializeField] Text metMapText;
    [SerializeField] Text metLevelText;
    [SerializeField] Text hpText;
    [SerializeField] Text statsText;
    [SerializeField] MoveSelectionUI moveSelection;
    [SerializeField] Image moveCategoryImage;
    [SerializeField] Text movePowerText;
    [SerializeField] Text moveAccuracyText;
    [SerializeField] Text moveDescriptionText;

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
        hpText.text = $"{pokemon.hp}/{pokemon.maxHp}";
    }

    public override void RefreshXP()
    {
        xpNextLevelText.text = pokemon.xpNeededForNextLevel.ToString();
        xpText.text = pokemon.xp.ToString();
        base.RefreshXP();
    }
}
