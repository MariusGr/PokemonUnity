using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSprite : MonoBehaviour
{
    [SerializeField] new Animation animation;

    private bool isPlayingAnimation;

    public void PlayAnimation(AnimationClip animationClip)
    {
        isPlayingAnimation = true;
        animation.clip = animationClip;
        animation.Play();
        StartCoroutine(WaitForAnimationFinish());
    }

    IEnumerator WaitForAnimationFinish()
    {
        yield return new WaitForSeconds(animation.clip.length);
        isPlayingAnimation = false;
    }

    public bool IsPlayingAnimation() => isPlayingAnimation;
}
