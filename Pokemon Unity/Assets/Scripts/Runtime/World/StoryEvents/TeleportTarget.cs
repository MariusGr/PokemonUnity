using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTarget : MonoBehaviour
{
    private static readonly Dictionary<string, TeleportTarget> keyToInstance = new Dictionary<string, TeleportTarget>();
    public static bool TryGet(string key, out TeleportTarget value) => keyToInstance.TryGetValue(key, out value);

    [SerializeField] private string key;
    [SerializeField] private Character targetNPC;
    [SerializeField] private Direction targetNPCFaceDirection;

    private void Awake() => keyToInstance.Add(key, this);

    public void TeleportTo(Direction playerFaceDirection)
    {
        PlayerCharacter.Instance.Teleport(transform.position, playerFaceDirection);
        targetNPC?.Movement.LookInDirection(targetNPCFaceDirection);
    }
}
