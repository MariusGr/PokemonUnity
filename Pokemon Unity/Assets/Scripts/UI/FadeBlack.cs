using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlack : AnimatedSprite
{
    [SerializeField] AnimationClip fadeToBlack;
    [SerializeField] AnimationClip fadeFromBlack;


    public IEnumerator FadeToBlack()
    {
        return PlayAnimation(fadeToBlack);
    }

    public IEnumerator FadeFromBlack()
    {
        return PlayAnimation(fadeFromBlack);
    }
}
