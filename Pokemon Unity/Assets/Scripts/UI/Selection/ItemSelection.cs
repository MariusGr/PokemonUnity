using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSelection : ScalarSelection
{
    [SerializeField] protected ShadowedText categoryText;

    protected ScalarSelection[] selections;
    protected ScalarSelection activeSelection => selections[selectedIndex];
}
