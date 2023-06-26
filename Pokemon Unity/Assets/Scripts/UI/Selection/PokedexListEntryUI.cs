using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokedexListEntryUI : ListEntryUI
{
    [SerializeField] protected ShadowedText dexText;

    [HideInInspector] public DexEntryData dexEntryData;

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);

        base.AssignElement(payload);

        dexEntryData = (DexEntryData)payload;

        dexText.text = dexEntryData.dex.ToString("000");

        if (dexEntryData.pokemon is null)
            nameText.text = "-----";
        else
            nameText.text = dexEntryData.pokemon.SpeciesName;

        icon.enabled = dexEntryData.caught;
    }
}
