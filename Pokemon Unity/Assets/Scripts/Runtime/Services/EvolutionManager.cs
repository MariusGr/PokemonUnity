using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager Instance;

    public EvolutionManager() => Instance = this;
    public void Evolve(Pokemon pokemon) => StartCoroutine(EvolutionCoroutine(pokemon));

    public IEnumerator EvolutionCoroutine(Pokemon pokemon)
    {
        Pokemon evolved = pokemon.GetEvolvedVersion();
        EvolutionUI.Instance.Open();
        yield return EvolutionUI.Instance.AnimateEvolution(pokemon, evolved);
        pokemon.character.ExchangePokemon(pokemon, evolved);
        // TODO Learn all moves of new evolution
        EvolutionUI.Instance.Close();
    }
}
