using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionUI : MonoBehaviour, IRegionUI
{
    [SerializeField] new Animation animation;
    [SerializeField] RectTransform boxTransform;
    [SerializeField] ShadowedText text;

    public RegionUI() => Services.Register(this as IRegionUI);

    private void Start()
    {
        animation.Play();
        animation.Stop();
    }

    public void ShowRegionName(string name)
    {
        text.text = name;
        animation.Stop();
        animation.Play();
    }
}
