using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCluster : EncounterArea
{
    public static Dictionary<GameObject, GrassCluster> ChildToInstanceMap = new Dictionary<GameObject, GrassCluster>();

    void Awake()
    {
        foreach (Transform t in transform)
            ChildToInstanceMap[t.gameObject] = this;

        Initialize();
    }
}
