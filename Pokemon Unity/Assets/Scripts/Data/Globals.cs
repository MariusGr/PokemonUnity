using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;

public class Globals : MonoBehaviour
{
    public static Globals Instance;

    public Gender[] genders;

    public InspectorFriendlySerializableDictionary<Status, Sprite> statusToSpriteMap;

    Globals()
    {
        Instance = this;
    }
}
