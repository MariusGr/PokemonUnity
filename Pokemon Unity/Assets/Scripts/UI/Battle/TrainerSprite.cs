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
        SetVisiblity(true);
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
        SetVisiblity(!disableAtEnd);
    }

    public void SetVisiblity(bool visivle) => spriteImage.enabled = visivle;
}
