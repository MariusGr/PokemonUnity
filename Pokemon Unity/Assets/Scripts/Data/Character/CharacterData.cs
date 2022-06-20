using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterData
{
    public string name;
    public string nameGenitive => name.Length > 0 && name[name.Length - 1] == 's' ? $"{name}'" : $"{name}s";
    public Pokemon[] pokemons;
}
