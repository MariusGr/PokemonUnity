using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvolutionManager : IService
{
    public void Evolve(IPokemon pokemon);
    public IEnumerator EvolutionCoroutine(IPokemon pokemon);
}
