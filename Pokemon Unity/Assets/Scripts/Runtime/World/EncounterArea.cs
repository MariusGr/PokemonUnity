using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterArea : MonoBehaviour
{
    private static EncounterArea currentArea;

    [SerializeField] float encounterChance = .2f;
    [SerializeField] EncounterPokemon[] pokemons;

    private Dictionary<EncounterRarity, List<EncounterPokemon>> rarityToPokemonMap = new Dictionary<EncounterRarity, List<EncounterPokemon>>();
    private Dictionary<EncounterRarity, float> rarityToValueMap = new Dictionary<EncounterRarity, float>();

    HashSet<EncounterRarity> givenRarities = new HashSet<EncounterRarity>();

    private void Awake() => Initialize();

    protected void Initialize()
    {
        foreach (EncounterPokemon p in pokemons)
        {
            givenRarities.Add(p.data.encounterRarity);
        }

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
        if (!(currentArea is null))
            currentArea.CheckEncounter();
    }

    public void Enter()
    {
        currentArea = this;
    }

    public void Leave()
    {
        currentArea = null;
    }

    public void CheckEncounter()
    {
        float f = UnityEngine.Random.value;
        // Does an encounter happen at all?
        if (f > encounterChance)
            return;

        // Choose current ancounter rarity randomly
        EncounterRarity currentRarity = EncounterRarity.Common;
        bool encounterFound = false;
        print(givenRarities.Count);
        foreach (EncounterRarity e in givenRarities)
        {
            float calue = UnityEngine.Random.value;
            print(calue + "   " + rarityToValueMap[e]);
            if (calue <= rarityToValueMap[e])
            {
                currentRarity = e;
                encounterFound = true;
            }
        }

        if (!encounterFound)
            return;

        // Get all pokemon with that rarity in this area
        List<EncounterPokemon> currentPokemons = new List<EncounterPokemon>();
        foreach (EncounterPokemon p in pokemons)
            if (p.data.encounterRarity == currentRarity)
                currentPokemons.Add(p);

        // Choose random pokemon with currentRarity
        EncounterPokemon chosenPokemon = currentPokemons[UnityEngine.Random.Range(0, currentPokemons.Count)];
        Pokemon pokemon = new Pokemon(chosenPokemon);

        // Start wild encounter
        BattleManager.Instance.StartNewEncounter(PlayerData.Instance, pokemon, EncounterEndReaction);
    }

    public bool EncounterEndReaction(bool wildPokemonDefeated)
    {
        DialogBox.Instance.Close();
        return true;
    }
}
