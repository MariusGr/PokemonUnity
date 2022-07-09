using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AnimatedSprite : MonoBehaviour
{
    [SerializeField] new Animation animation;
    [SerializeField] protected Image spriteImage;

    private bool isPlayingAnimation;

    public void SetSprite(Sprite sprite) => spriteImage.sprite = sprite;

    public void PlayAnimation(AnimationClip clip)
    {
        isPlayingAnimation = true;
        animation.clip = clip;
        animation.Play();
        StartCoroutine(WaitForAnimationFinish());
    }

    virtual protected IEnumerator WaitForAnimationFinish()
    {
        yield return new WaitForSeconds(animation.clip.length);
        isPlayingAnimation = false;
    }

    public bool IsPlayingAnimation() => isPlayingAnimation;
}
