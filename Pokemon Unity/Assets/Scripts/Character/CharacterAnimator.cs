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
    [SerializeField] Sprite[] walkSpritesLeft;
    [SerializeField] Sprite[] walkSpritesUp;
    [SerializeField] Sprite[] walkSpritesDown;

    public float interval;
    Dictionary<Tuple<AnimationType, Direction>, Sprite[]> animationSpriteMap;
    Dictionary<AnimationType, float> animationTypeToInterval;
    Coroutine currentAnimation;
    Sprite[] currentImageSequence;
    int currentSpriteIndex = 0;
    bool animationRunning = false;

    private void Start()
    {
        animationSpriteMap = new Dictionary<Tuple<AnimationType, Direction>, Sprite[]>()
        {
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Right
            ), walkSpritesRight },
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Left
            ), walkSpritesLeft },
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Up
            ), walkSpritesUp },
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Down
            ), walkSpritesDown },

            { new Tuple<AnimationType, Direction>(
                AnimationType.Walk, Direction.Right
            ), walkSpritesRight },
            { new Tuple<AnimationType, Direction>(
                AnimationType.Walk, Direction.Left
            ), walkSpritesLeft },
            { new Tuple<AnimationType, Direction>(
                AnimationType.Walk, Direction.Up
            ), walkSpritesUp },
            { new Tuple<AnimationType, Direction>(
                AnimationType.Walk, Direction.Down
            ), walkSpritesDown },
        };
        animationTypeToInterval = new Dictionary<AnimationType, float>()
        {
            { AnimationType.Walk, .1f },
        };

        currentImageSequence = walkSpritesDown;
    }

    public void Refresh(AnimationType animation, Direction direction)
    {
        currentImageSequence = animationSpriteMap[new Tuple<AnimationType, Direction>(animation, direction)];
        renderer.sprite = currentImageSequence[currentSpriteIndex];
        if (animation == AnimationType.None || direction == Direction.None)
            StopCycle();
        else
            StartCycle();
    }

    void StartCycle()
    {
        animationRunning = true;
        if (currentAnimation == null)
            currentAnimation = StartCoroutine(Animation(interval));
    }

    void StopCycle() => animationRunning = false;

    IEnumerator Animation(float interval)
    {
        currentSpriteIndex = 0;
        
        while (true)
        {
            if (!animationRunning && currentSpriteIndex % 2 == 0)
            {
                currentSpriteIndex = 0;
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }

            yield return new WaitForSeconds(interval);
            currentSpriteIndex = (currentSpriteIndex + 1) % currentImageSequence.Length;
        }
    }
}
