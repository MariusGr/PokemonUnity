using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    static public LayerManager Instance;

    public LayerManager() => Instance = Instance ?? this;

    static public void SetLayerRecursively(GameObject obj, int newLayer)
        => SetLayerRecursively(obj, newLayer, new List<Transform>());

    static public void SetLayerRecursively(GameObject obj, int newLayer, List<Transform> skip)
    {
        if (skip == null || !skip.Contains(obj.transform))
            obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer, skip);
    }

    [field: SerializeField] public LayerMask DefaultLayerMask { get; private set; }
    [field: SerializeField] public LayerMask GroundLayerMask { get; private set; }
    [field: SerializeField] public LayerMask MovementBlockingLayerMask { get; private set; }
    [field: SerializeField] public LayerMask InteractableLayerMask { get; private set; }
    [field: SerializeField] public LayerMask PlayerLayerMask { get; private set; }
    [field: SerializeField] public LayerMask VisionBlockingForAILayerMask { get; private set; }
    [field: SerializeField] public LayerMask EntranceLayerMask { get; private set; }
    [field: SerializeField] public LayerMask FollowerLayerMask { get; private set; }

    public static int FollowerLayer { get; private set; }

    public static int ToLayer(int bitmask)
    {
        int result = bitmask > 0 ? 0 : 31;
        while (bitmask > 1)
        {
            bitmask = bitmask >> 1;
            result++;
        }
        return result;
    }

    private void Awake()
    {
        FollowerLayer = ToLayer(FollowerLayerMask);
    }
}
