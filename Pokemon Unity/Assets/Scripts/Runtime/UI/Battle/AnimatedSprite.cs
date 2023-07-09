using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AnimatedSprite : MonoBehaviour
{
    [SerializeField] new Animation animation;
    [SerializeField] protected Image spriteImage;

    private bool isPlayingAnimation;

    public void SetSprite(Sprite sprite)
    {
        ResetAnimation();
        spriteImage.sprite = sprite;
    }

    public IEnumerator PlayAnimation(AnimationClip clip)
    {
        if (animation.GetClip(clip.name) == null)
            animation.AddClip(clip, clip.name);
        isPlayingAnimation = true;
        animation.clip = clip;
        animation.Play();
        yield return WaitForAnimationFinish();
    }

    virtual protected IEnumerator WaitForAnimationFinish()
    {
        yield return new WaitForSeconds(animation.clip.length);
        isPlayingAnimation = false;
    }

    public void ResetAnimation()
    {
        isPlayingAnimation = false;
        animation.Rewind();
        animation.Play();
        animation.Sample();
        animation.Stop();
        print("Play Idle");
    }

    public IEnumerator PlayBlinkAnimation(float duration = 1f, int times = 3)
    {
        isPlayingAnimation = true;
        float interval = duration / ((float)times * 2f);

        for (int i = 0; i < times * 2; i++)
        {
            spriteImage.enabled = !spriteImage.enabled;
            yield return new WaitForSeconds(interval);
        }

        isPlayingAnimation = false;
    }

    public bool IsPlayingAnimation() => isPlayingAnimation;
}
