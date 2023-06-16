using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove : IJSONConvertable
{
    public IMoveData Data { get; }
    public int Index { get; set; }
    public IPokemon pokemon { get; }
    public int Pp { get; }
    public bool IsBlocked { get; }

    public bool IsUsable { get; }

    public int GetDamageAgainst(IPokemon attacker, IPokemon target, out bool critcal, out Effectiveness effectiveness);
    public bool IsFaster(IMove other);
    public void DecrementPP();
    public void ReplenishPP();
    public void SetPokemon(IPokemon pokemon);
    public bool TryHit(IPokemon attacker, IPokemon target, out FailReason failReason);
}
