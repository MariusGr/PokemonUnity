using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveCategory
{
    public string Name { get; }
    public Sprite Icon { get; }
    public bool IsSpecial { get; }
}
