using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerSprite : AnimatedSprite
{
    [SerializeField] AnimationClip appearAnimation;
    [SerializeField] AnimationClip disappearAnimation;

    public void PlayAppearAnimation()
    {
        spriteImage.enabled = true;
        PlayAnimation(appearAnimation);
    }

    public void PlayDisappearAnimation() => PlayAnimation(disappearAnimation);

    protected override IEnumerator WaitForAnimationFinish()
    {
        yield return base.WaitForAnimationFinish();
        spriteImage.enabled = !spriteImage.enabled;
    }
}
