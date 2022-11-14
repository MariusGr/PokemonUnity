using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvolutionManager : IService
{
    public void Evolve(Pokemon pokemon);
    public IEnumerator EvolutionCoroutine(Pokemon pokemon);
}
