using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerSprite : AnimatedSprite
{
    [SerializeField] AnimationClip appearAnimation;
    [SerializeField] AnimationClip disappearAnimation;

    bool disableAtEnd = false;

    public IEnumerator PlayAppearAnimation()
    {
        disableAtEnd = false;
        SetVisiblity(true);
        yield return PlayAnimation(appearAnimation);
    }

    public IEnumerator PlayDisappearAnimation()
    {
        disableAtEnd = true;
        yield return PlayAnimation(disappearAnimation);
    }

    protected override IEnumerator WaitForAnimationFinish()
    {
        yield return base.WaitForAnimationFinish();
        SetVisiblity(!disableAtEnd);
    }

    public void SetVisiblity(bool visivle) => spriteImage.enabled = visivle;
}
