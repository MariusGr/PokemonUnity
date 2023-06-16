using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

[Serializable]
public class NPCData : CharacterData, INPCData
{
    public Sprite sprite;
    public string defaultText;
    public string challengeText;
    public string battleDefeatText;
    public string battleWinText;
    public string afterDefeatText;
    public string defeatedText;
    public float priceMoneyBase = 0;
    public bool hasBeenDefeated = false;

    override public float GetPriceMoney() => Pokemons[Pokemons.Count - 1].Level * priceMoneyBase;

    // TODO take start position
    public string GetKey()
        => $"{GetType()}_{Name}_{Gameobject.transform.position.x}_{Gameobject.transform.position.y}_{Gameobject.transform.position.z}";
}
