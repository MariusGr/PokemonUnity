using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveCategory", menuName = "Pokemon/MoveCategory")]
public class MoveCategory : ScriptableObject, IMoveCategory
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public bool IsSpecial { get; private set; }
}
