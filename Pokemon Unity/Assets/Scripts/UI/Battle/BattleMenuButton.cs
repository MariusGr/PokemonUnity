using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuButton : SelectableImage
{
    [SerializeField] public BattleOption option;

    private void Awake()
    {
        Initialize();
        AssignElement(null);
    }
}
