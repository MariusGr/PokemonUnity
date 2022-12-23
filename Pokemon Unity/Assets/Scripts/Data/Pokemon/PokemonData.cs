using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[CreateAssetMenu(fileName = "PokemonData", menuName = "Pokemon/Pokemon Data")]
public class PokemonData : BaseScriptableObject
{
    public static int Count { get; private set; } = 0;

    public int dex;
    public string fullName;
    public string description;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite[] icons;
    public Sprite icon => icons[0];
    public PokemonTypeData[] pokemonTypes;
    public int evolutionLevel;
    public PokemonData evolution;
    public EncounterRarity encounterRarity;
    public int catchRate;
    public AudioClip cry => cries[Random.Range(0, cries.Length)];
    public AudioClip faintCry => faintCries.Length > 0 ? faintCries[Random.Range(0, faintCries.Length)] : cry;
    [SerializeField] private AudioClip[] cries;
    [SerializeField] private AudioClip[] faintCries;

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
