using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionUI : MonoBehaviour
{
    public static RegionUI Instance;

    [SerializeField] new Animation animation;
    [SerializeField] RectTransform boxTransform;
    [SerializeField] ShadowedText text;

    public RegionUI() => Instance = this;

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
