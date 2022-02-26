using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    None,
    Walk,
}

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] new SpriteRenderer renderer;
    [SerializeField] Sprite[] walkSpritesRight;

    public float interval;
    Dictionary<Tuple<AnimationType, Direction>, Sprite[]> animationSpriteMap;
    Dictionary<AnimationType, float> animationTypeToInterval;
    Coroutine currentAnimation;

    private void Start()
    {
        animationSpriteMap = new Dictionary<Tuple<AnimationType, Direction>, Sprite[]>()
        {
            { new Tuple<AnimationType, Direction>(
                AnimationType.Walk, Direction.Right
            ), walkSpritesRight },
        };
        animationTypeToInterval = new Dictionary<AnimationType, float>()
        {
            { AnimationType.Walk, .1f },
        };
    }

    public void Refresh(AnimationType animation, GridVector direction)
    {
        if (animation == AnimationType.None)
            StopCycle();
        else
            StartCycle(animation, direction);
    }

    void StartCycle(AnimationType animation, GridVector direction)
        => StartCycle(animation, direction.ToDirection());

    void StartCycle(AnimationType animation, Direction direction)
    {
        if (currentAnimation != null)
            currentAnimation = StartCoroutine(Animation(
                animationSpriteMap[new Tuple<AnimationType, Direction>(animation, direction)],
                interval));
    }

    void StopCycle()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
    }

    IEnumerator Animation(Sprite[] imageSequence, float interval)
    {
        int i = 0;
        while(true)
        {
            renderer.sprite = imageSequence[i];
            yield return new WaitForSeconds(interval);
        }
    }
}
