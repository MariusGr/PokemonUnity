using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum Statusxx
{
    None = 0,
    Sleeping = 1,
    Frozen = 2,
    Piosened = 4,
    Paralyzed = 8,
    Burned = 16,
    Confused = 32,
    Fainted = 64,
}
