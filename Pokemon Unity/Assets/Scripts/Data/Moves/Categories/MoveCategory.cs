using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveCategory", menuName = "Pokemon/MoveCategory")]
public class MoveCategory : ScriptableObject
{
    public string fullName;
    public Sprite icon;
    public bool isSpecial;
}
