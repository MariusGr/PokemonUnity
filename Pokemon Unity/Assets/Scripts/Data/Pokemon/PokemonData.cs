using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PokemonData", menuName = "Pokemon/Pokemon Data")]
public class PokemonData : ScriptableObject
{
    public int dex;
    public string fullName;
    public string description;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite[] icons;
    public PokemonTypeData[] pokemonTypes;
    //public PokemonData evolution;
    public EncounterRarity encounterRarity;

    public int maxHp;
    public int attack;
    public int specialAttack;
    public int defense;
    public int specialDefense;
    public int speed;

    public CollectionExtensions.IntMoveDataDictionary levelToMoveDataMap;

    public Sprite GetBattleSprite(int characterIndex) => characterIndex == Constants.PlayerIndex ? backSprite : frontSprite;
}
