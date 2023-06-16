using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPokemonData
{
    public string Name { get; }
    public string Id { get; }
    public int Dex { get; }
    public string FullName { get; }
    public string Description { get; }
    public Sprite FrontSprite { get; }
    public Sprite BackSprite { get; }
    public Sprite[] Icons { get; }
    public Sprite Icon { get; }
    public IPokemonTypeData[] PokemonTypes { get; }
    public int EvolutionLevel { get; }
    public IPokemonData Evolution { get; }
    public EncounterRarity EncounterRarity { get; }
    public int CatchRate { get; }
    public AudioClip Cry { get; }
    public AudioClip FaintCry { get; }

    public int MaxHp { get; }
    public int Attack { get; }
    public int SpecialAttack { get; }
    public int Defense { get; }
    public int SpecialDefense { get; }
    public int Speed { get; }
    public int BaseXPGain { get; }


    public CollectionExtensions.IntMoveDataDictionary LevelToMoveDataMap { get; }

    public Sprite GetBattleSprite(int characterIndex);
    public Sprite GetType2Sprite();
    public int GetXPForLevel(int level);
    public bool GetMoveLearnedAtLevel(int level, out IMoveData move);
}
