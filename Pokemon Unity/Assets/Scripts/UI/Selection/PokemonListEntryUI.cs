using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonListEntryUI : ListEntryUI
{
    [SerializeField] protected ShadowedText dexText;

    [HideInInspector] public PokemonData pokemon;

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);

        base.AssignElement(payload);

        pokemon = (PokemonData)payload;
        dexText.text = $"#{pokemon.dex}";
        icon.sprite = pokemon.icons[0];
        nameText.text = pokemon.fullName;
    }
}
