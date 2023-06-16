using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour, IEvolutionManager
{
    private IEvolutionUI ui;

    public EvolutionManager() => Services.Register(this as IEvolutionManager);
    private void Awake() => ui = Services.Get<IEvolutionUI>();
    public void Evolve(IPokemon pokemon) => StartCoroutine(EvolutionCoroutine(pokemon));

    public IEnumerator EvolutionCoroutine(IPokemon pokemon)
    {
        IPokemon evolved = pokemon.GetEvolvedVersion();
        ui.Open();
        yield return ui.AnimateEvolution(pokemon, evolved);
        pokemon.Character.ExchangePokemon(pokemon, evolved);
        // TODO Learn all moves of new evolution
        ui.Close();
    }
}
