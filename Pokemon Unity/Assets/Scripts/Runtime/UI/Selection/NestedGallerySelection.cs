using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestedGallerySelection : ScalarSelection
{
    [SerializeField] protected ShadowedText pageTitleText;

    protected ScalarSelection[] selections;
    protected ScalarSelection activeSelection => selections[selectedIndex];
}
