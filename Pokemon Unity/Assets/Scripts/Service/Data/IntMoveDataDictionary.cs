using UnityEngine;
using System;
using System.Collections.Generic;
using AYellowpaper;

namespace CollectionExtensions
{
    [Serializable]
    public class IntMoveDataDictionary : DictionaryWithInterfaceTypeValues<int, IMoveData, ScriptableObject> { }
}
