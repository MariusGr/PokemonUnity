using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PokemonData", menuName = "Pokemon/Pokemon Data")]
public class PokemonData : ScriptableObject
{
    public static int Count { get; private set; } = 0;

    public int dex;
    public string fullName;
    public string description;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite[] icons;
    public PokemonTypeData[] pokemonTypes;
    public int evolutionLevel;
    public PokemonData evolution;
    public EncounterRarity encounterRarity;
    public int catchRate;

    public int maxHp;
    public int attack;
    public int specialAttack;
    public int defense;
    public int specialDefense;
    public int speed;
    public int baseXPGain;

    public CollectionExtensions.IntMoveDataDictionary levelToMoveDataMap;

    public Sprite GetBattleSprite(int characterIndex) => characterIndex == Constants.PlayerIndex ? backSprite : frontSprite;
    public Sprite GetType2Sprite() => pokemonTypes.Length > 1 ? pokemonTypes[1].titleSprite : null;
    public int GetXPForLevel(int level) => (int)Mathf.Pow(level, 3);

    public PokemonData() => Count++;
}
