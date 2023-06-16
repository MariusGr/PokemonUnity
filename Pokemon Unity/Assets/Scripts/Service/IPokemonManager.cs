using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPokemonManager : IService
{
    public IEnumerator GrowLevel(IPokemon pokemon, System.Action<bool> uiRefreshCallback);
    public IEnumerator TryLearnMove(IPokemon pokemon, IMoveData move);
    public Coroutine TryUseItemOnPokemon(IItem item, IPokemon pokemon, IEnumerator animation, System.Action<bool> success);
}
