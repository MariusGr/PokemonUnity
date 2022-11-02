using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonPartyViewStatsUI : PlayerPokemonStatsUI
{
    Sprite backgroundDefault;
    Sprite backgroundDefaultSelected;
    [SerializeField] Sprite backgroundFaint;
    [SerializeField] Sprite backgroundFaintSelected;

    public override void Initialize(int index)
    {
        base.Initialize(index);
        backgroundDefault = spriteBefore;
        backgroundDefaultSelected = selectedSprite;
    }

    public override void Refresh()
    {
        base.Refresh();
        if (pokemon.isFainted)
            SetBackgroundToFainted();
        else
            SetBackgroundToDefault();
    }

    public void SetBackgroundToDefault()
    {
        spriteBefore = backgroundDefault;
        selectedSprite = backgroundDefaultSelected;
        image.sprite = currentSprite;
    }

    public void SetBackgroundToFainted()
    {
        spriteBefore = backgroundFaint;
        selectedSprite = backgroundFaintSelected;
        image.sprite = currentSprite;
    }
}
