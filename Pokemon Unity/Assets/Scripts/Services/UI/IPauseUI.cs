using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPauseUI : IUIView
{
    public void Assign(CharacterData player);
}
