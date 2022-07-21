using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerSprite : AnimatedSprite
{
    [SerializeField] AnimationClip appearAnimation;
    [SerializeField] AnimationClip disappearAnimation;

    bool disableAtEnd = false;

    public void PlayAppearAnimation()
    {
        disableAtEnd = false;
        spriteImage.enabled = true;
        PlayAnimation(appearAnimation);
    }

    public void PlayDisappearAnimation()
    {
        disableAtEnd = true;
        PlayAnimation(disappearAnimation);
    }

    protected override IEnumerator WaitForAnimationFinish()
    {
        yield return base.WaitForAnimationFinish();
        spriteImage.enabled = !disableAtEnd;
    }
}
