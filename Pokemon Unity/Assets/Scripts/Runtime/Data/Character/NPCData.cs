using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SimpleJSON;

[Serializable]
public class NPCData : CharacterData
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

    override public float GetPriceMoney() => pokemons[pokemons.Count - 1].level * priceMoneyBase;

    // TODO take start position
    public string GetKey()
        => $"{GetType()}_{name}_{gameobject.transform.position.x}_{gameobject.transform.position.y}_{gameobject.transform.position.z}";
}
