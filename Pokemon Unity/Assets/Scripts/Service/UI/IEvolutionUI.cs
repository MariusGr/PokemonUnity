using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvolutionUI : IUIView
{
    public IEnumerator AnimateEvolution(IPokemon before, IPokemon after);
}
