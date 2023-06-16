using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokedexSelection : ScrollSelection
{
    [SerializeField] ShadowedText selectedName;
    [SerializeField] Image selectedSprite;
    [SerializeField] Image selectedType1;
    [SerializeField] Image selectedType2;
    [SerializeField] Sprite unknownType;

    public override void Open(Action<ISelectableUIElement, bool> callback, bool forceSelection, int startSelection)
    {
        DexEntryData[] dexList = new DexEntryData[PokemonData.Count];
        for (int i = 0; i < dexList.Length; i++)
            dexList[i] = new DexEntryData(i + 1);

        foreach (PokemonData pokemon in PlayerData.Instance.caughtPokemons)
        {
            DexEntryData data = dexList[pokemon.Dex - 1];
            data.pokemon = pokemon;
            data.pokemon = pokemon;
            data.seen = true;
            data.caught = true;
        }

        foreach (PokemonData pokemon in PlayerData.Instance.seenPokemons)
        {
            DexEntryData data = dexList[pokemon.Dex - 1];
            if (data.caught)
                continue;
            data.pokemon = pokemon;
            data.seen = true;
        }

        AssignItems(dexList);
        base.Open(callback, forceSelection, startSelection);
    }

    protected override void SelectElement(int index)
    {
        base.SelectElement(index);

        DexEntryData data = ((PokedexListEntryUI)selectedElement).dexEntryData;
        selectedName.text = data.pokemon is null ? "???" : data.pokemon.FullName;

        description.gameObject.SetActive(data.caught);
        selectedSprite.enabled = data.seen;
        selectedType1.enabled = data.seen;
        selectedType2.enabled = data.seen && data.pokemon.PokemonTypes.Length > 1 && !(data.pokemon.PokemonTypes[1] is null);
        selectedSprite.sprite = data.pokemon is null ? null : data.pokemon.FrontSprite;

        selectedType1.sprite = data.caught ? data.pokemon.PokemonTypes[0].TitleSprite : unknownType;
        selectedType2.sprite = data.caught && selectedType2.enabled ? data.pokemon.PokemonTypes[1].TitleSprite : unknownType;
        description.text = data.pokemon is null ? "" : data.pokemon.Description;
    }
}
