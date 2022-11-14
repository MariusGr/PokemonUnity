using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour, IEvolutionManager
{
    private IEvolutionUI ui;

    public EvolutionManager() => Services.Register(this as IEvolutionManager);

    private void Awake()
    {
        ui = Services.Get<IEvolutionUI>();
    }

    public void Evolve(Pokemon pokemon)
    {
        StartCoroutine(EvolutionCoroutine(pokemon));
    }

    public IEnumerator EvolutionCoroutine(Pokemon pokemon)
    {
        Pokemon evolved = pokemon.GetEvolvedVersion();
        ui.Open();
        yield return ui.AnimateEvolution(pokemon, evolved);
        pokemon.character.SwapPokemon(pokemon, evolved);
        ui.Close();
    }
}
