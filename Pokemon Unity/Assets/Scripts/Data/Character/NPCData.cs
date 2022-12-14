using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
}
