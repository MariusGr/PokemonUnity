using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonListEntryUI : ListEntryUI
{
    [HideInInspector] public Pokemon pokemon;

    public override void AssignElement(object payload)
    {
        gameObject.SetActive(true);
        base.AssignElement(payload);

        pokemon = (Pokemon)payload;
        nameText.text = pokemon.Name;
        icon.enabled = pokemon.data.icon;
    }
}
