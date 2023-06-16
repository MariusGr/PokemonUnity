using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

[CreateAssetMenu(fileName = "PokemonData", menuName = "Pokemon/Pokemon Data")]
public class PokemonData : BaseScriptableObject, IPokemonData
{
    public static int Count { get; private set; } = 0;

    public string Name => name;
    [field: SerializeField] public int Dex { get; private set; }
    [field: SerializeField] public string FullName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite FrontSprite { get; private set; }
    [field: SerializeField] public Sprite BackSprite { get; private set; }
    [field: SerializeField] public Sprite[] Icons { get; private set; }
    [field: SerializeField] public IPokemonTypeData[] PokemonTypes { get; private set; }
    [field: SerializeField] public int EvolutionLevel { get; private set; }
    [field: SerializeField] public IPokemonData Evolution { get; private set; }
    [field: SerializeField] public EncounterRarity EncounterRarity { get; private set; }
    [field: SerializeField] public int CatchRate { get; private set; }

    public Sprite Icon => Icons[0];
    public AudioClip Cry => cries[Random.Range(0, cries.Length)];
    public AudioClip FaintCry => faintCries.Length > 0 ? faintCries[Random.Range(0, faintCries.Length)] : Cry;

    [SerializeField] private AudioClip[] cries;
    [SerializeField] private AudioClip[] faintCries;

    [field: SerializeField] public int MaxHp { get; private set; }
    [field: SerializeField] public int Attack { get; private set; }
    [field: SerializeField] public int SpecialAttack { get; private set; }
    [field: SerializeField] public int Defense { get; private set; }
    [field: SerializeField] public int SpecialDefense { get; private set; }
    [field: SerializeField] public int Speed { get; private set; }
    [field: SerializeField] public int BaseXPGain { get; private set; }

    [field: SerializeField] public CollectionExtensions.IntMoveDataDictionary LevelToMoveDataMap { get; private set; }

    public Sprite GetBattleSprite(int characterIndex) => characterIndex == Constants.PlayerIndex ? BackSprite : FrontSprite;
    public Sprite GetType2Sprite() => PokemonTypes.Length > 1 ? PokemonTypes[1].TitleSprite : null;
    public int GetXPForLevel(int level) => (int)Mathf.Pow(level, 3);

    public PokemonData() => Count++;

    public bool GetMoveLearnedAtLevel(int level, out IMoveData move)
    {
        IMoveData m;
        bool success = LevelToMoveDataMap.TryGetValue(level, out m);
        move = m;
        return success;
    }
}
