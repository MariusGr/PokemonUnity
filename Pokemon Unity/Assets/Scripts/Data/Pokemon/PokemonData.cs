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
    public PokemonTypeData[] pokemonTypes;
    public PokemonData evolution;

    public float hp;
    public float attack;

    public CollectionExtensions.IntMoveDataDictionary levelToMoveMap;
}
