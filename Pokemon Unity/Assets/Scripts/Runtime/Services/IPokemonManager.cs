using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPokemonManager : IService
{
    public IEnumerator GrowLevel(Pokemon pokemon, System.Action<bool> uiRefreshCallback);
    public IEnumerator TryLearnMove(Pokemon pokemon, MoveData move);
}
