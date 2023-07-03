using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;


[CreateAssetMenu(fileName = "NewPokemonType", menuName = "Pokemon/Pokemon Type")]
public class PokemonTypeData : ScriptableObject, IPokemonTypeData
{
    [field: SerializeField] public string Name { get; private set; } = "Type";
    [field: SerializeField] public Sprite TitleSprite { get; private set; }
    [field: SerializeField] public Color Color { get; private set; } = Color.white;

    public DictionaryWithInterfaceTypeKeys<IPokemonTypeData, float, ScriptableObject> effectiveness;

    public float GetEffectiveness(IPokemonTypeData againstType)
    {
        if (effectiveness.keys.Contains(againstType))
            return effectiveness[againstType];
        return 1f;
    }

    public float GetEffectiveness(IPokemon pokemon)
    {
        float effectiveness = 1f;
        foreach (PokemonTypeData type in pokemon.Data.PokemonTypes)
            effectiveness *= GetEffectiveness(type);
        return effectiveness;
    }
}
