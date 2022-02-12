using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    static public LayerManager Instance;

    static public void SetLayerRecursively(GameObject obj, int newLayer)
        => SetLayerRecursively(obj, newLayer, new List<Transform>());

    static public void SetLayerRecursively(GameObject obj, int newLayer, List<Transform> skip)
    {
        if (skip == null || !skip.Contains(obj.transform))
            obj.layer = newLayer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer, skip);
    }

    [SerializeField] LayerMask defaultLayerMask;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] LayerMask movementBlockingLayerMask;

    public LayerMask DefaultLayerMask => defaultLayerMask;
    public LayerMask GroundLayerMask => groundLayerMask;
    public LayerMask MovementBlockingLayerMask => movementBlockingLayerMask;

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

    void Awake()
    {
        Instance = Instance ?? this;
    }
}
