using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvolutionUI : IUIView
{
    public IEnumerator AnimateEvolution(Pokemon before, Pokemon after);
}
