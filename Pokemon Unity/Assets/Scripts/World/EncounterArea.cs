using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterArea : MonoBehaviour
{
    private static EncounterArea currentArea;

    [SerializeField] float encounterChance = .2f;

    public EncounterPokemon[] pokemons;

    private Dictionary<EncounterRarity, List<EncounterPokemon>> rarityToPokemonMap = new Dictionary<EncounterRarity, List<EncounterPokemon>>();
    private Dictionary<EncounterRarity, float> rarityToValueMap = new Dictionary<EncounterRarity, float>();

    HashSet<EncounterRarity> givenRarities = new HashSet<EncounterRarity>();

    private void Awake()
    {
        foreach (EncounterPokemon p in pokemons)
            givenRarities.Add(p.data.encounterRarity);

        foreach (EncounterRarity e in givenRarities)
        {
            rarityToPokemonMap[e] = new List<EncounterPokemon>();
            rarityToValueMap[e] = (float)e / 100f;
        }

        foreach (EncounterPokemon p in pokemons)
            rarityToPokemonMap[p.data.encounterRarity].Add(p);
    }

    // TODO: should this be here in this class? Maybe own class!
    public static void CheckPositionRelatedEvents()
    {
        if (CharacterControllerAI.CheckAllNPCVision())
            return;
        currentArea.CheckEncounter();
    }

    public void Enter()
    {
        currentArea = this;
    }

    public void CheckEncounter()
    {
        // Does an encounter happen at all?
        if (UnityEngine.Random.value > encounterChance)
            return;

        // Choose current ancounter rarity randomly
        EncounterRarity currentRarity = givenRarities.GetEnumerator().Current;
        foreach (EncounterRarity e in givenRarities)
        {
            if (UnityEngine.Random.value <= rarityToValueMap[e])
                currentRarity = e;
        }

        // Get all pokemon with that rarity in this area
        List<EncounterPokemon> currentPokemons = new List<EncounterPokemon>();
        foreach (EncounterPokemon p in pokemons)
            if (p.data.encounterRarity == currentRarity)
                currentPokemons.Add(p);

        // Choose random pokemon with currentRarity
        EncounterPokemon chosenPokemon = currentPokemons[UnityEngine.Random.Range(0, currentPokemons.Count)];
        Pokemon pokemon = new Pokemon(chosenPokemon);

        // Start wild encounter
        Services.Get<BattleManager>().StartNewEncounter(Character.PlayerCharacter.characterData, pokemon);
    }
}
