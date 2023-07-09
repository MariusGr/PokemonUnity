using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlack : AnimatedSprite
{
    public static FadeBlack Instance;

    [SerializeField] AnimationClip fadeToBlack;
    [SerializeField] AnimationClip fadeFromBlack;

    FadeBlack() => Instance = this;
    public IEnumerator FadeToBlack() => PlayAnimation(fadeToBlack);
    public IEnumerator FadeFromBlack() => PlayAnimation(fadeFromBlack);
}
