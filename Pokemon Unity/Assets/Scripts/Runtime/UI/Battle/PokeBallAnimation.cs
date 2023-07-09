using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeBallAnimation : AnimatedSprite
{
    [SerializeField] AnimationClip throwAnimation;
    [SerializeField] AnimationClip shakeAnimation;
    [SerializeField] AudioClip shakeSound;
    [SerializeField] AudioClip throwSound;

    public IEnumerator PlayShakeAnimation()
    {
        SfxHandler.Play(shakeSound);
        return PlayAnimation(shakeAnimation);
    }

    public IEnumerator PlayThrowAnimation()
    {
        SfxHandler.Play(throwSound);
        return PlayAnimation(throwAnimation);
    }
}
