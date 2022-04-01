using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PokemonData", menuName = "Pokemon/Pokemon Data")]
public class PokemonData : ScriptableObject
{
    public int dex;
    public string fullName;
    public string description;
    public Texture2D backSprite;
    public PokemonType[] pokemonTypes = new PokemonType[] { PokemonType.Normal };
    public PokemonData evolution;

    public float hp;
    public float attack;
}
