using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPokemonType", menuName = "Pokemon/Pokemon Type")]
public class PokemonTypeData : ScriptableObject
{
    public string fullName = "Type";
    public Sprite titleSprite;
    public Color color = Color.white;
}
