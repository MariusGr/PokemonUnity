using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPokemonTypeData
{
    public string Name { get; }
    public Sprite TitleSprite { get; }
    public Color Color { get; }

    public float GetEffectiveness(IPokemonTypeData againstType);
    public float GetEffectiveness(IPokemon pokemon);
}
