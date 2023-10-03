using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CollectionExtensions;

[Serializable]
public class NpcData : CharacterData
{
    public Sprite sprite;
    [SerializeField] private string defaultDialogText;
    [SerializeField] private InspectorFriendlySerializableDictionary<StoryEvent, string> storyEventToDialogText; 
    public string challengeText;
    public string battleDefeatText;
    public string battleWinText;
    public string afterDefeatText;
    public string defeatedText;
    public float priceMoneyBase = 0;
    public bool hasBeenDefeated = false;
    public bool wantsToBattle;
    public float challengeVisionDistance = 5f;
    public AudioClip challengeMusicTrack;

    override public float GetPriceMoney() => pokemons[pokemons.Count - 1].level * priceMoneyBase;

    // TODO take start position
    public string GetKey()
        => $"{GetType()}_{name}_{gameobject.transform.position.x}_{gameobject.transform.position.y}_{gameobject.transform.position.z}";

    public string GetDialogText()
    {
        foreach (var entry in storyEventToDialogText)
            if (entry.Key.Happened)
                return entry.Value;
        return defaultDialogText;
    }
}
