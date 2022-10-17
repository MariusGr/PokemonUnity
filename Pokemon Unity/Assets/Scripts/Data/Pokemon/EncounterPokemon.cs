using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EncounterPokemon : MonoBehaviour
{
    public PokemonData data;
    public int minLevel = 2;
    public int maxLevel = 3;
}
