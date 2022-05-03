using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;


[CreateAssetMenu(fileName = "NewPokemonType", menuName = "Pokemon/Pokemon Type")]
public class PokemonTypeData : ScriptableObject
{
    public string fullName = "Type";
    public Sprite titleSprite;
    public Color color = Color.white;

    public InspectorFriendlySerializableDictionary<PokemonTypeData, float> effectiveness;

    public float GetEffectiveness(PokemonTypeData againstType)
    {
        if (effectiveness.keys.Contains(againstType))
            return effectiveness[againstType];
        return 1f;
    }

    public float GetEffectiveness(Pokemon pokemon)
    {
        float effectiveness = 1f;
        foreach (PokemonTypeData type in pokemon.data.pokemonTypes)
            effectiveness *= GetEffectiveness(type);
        return effectiveness;
    }
}
