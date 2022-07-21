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

    public void PlayBlinkAnimation(float duration = 1f, int times = 3) => StartCoroutine(BlinkRoutine(duration, times));

    IEnumerator BlinkRoutine(float duration, int times)
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
