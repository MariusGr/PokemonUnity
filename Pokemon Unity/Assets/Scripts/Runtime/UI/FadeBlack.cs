using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlack : AnimatedSprite, IFadeBlack
{
    [SerializeField] AnimationClip fadeToBlack;
    [SerializeField] AnimationClip fadeFromBlack;

    FadeBlack() => Services.Register(this as IFadeBlack);

    public IEnumerator FadeToBlack()
    {
        return PlayAnimation(fadeToBlack);
    }

    public IEnumerator FadeFromBlack()
    {
        return PlayAnimation(fadeFromBlack);
    }
}
