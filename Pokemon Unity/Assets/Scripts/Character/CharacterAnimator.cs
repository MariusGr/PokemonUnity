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

    [SerializeField] Sprite[] idleSpritesRight;
    [SerializeField] Sprite[] idleSpritesLeft;
    [SerializeField] Sprite[] idleSpritesUp;
    [SerializeField] Sprite[] idleSpritesDown;

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
    AnimationType currentAnimationType = AnimationType.None;
    Direction currentDirection = Direction.None;

    private void Start()
    {
        animationSpriteMap = new Dictionary<Tuple<AnimationType, Direction>, Sprite[]>()
        {
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Right
            ), idleSpritesRight },
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Left
            ), idleSpritesLeft },
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Up
            ), idleSpritesUp },
            { new Tuple<AnimationType, Direction>(
                AnimationType.None, Direction.Down
            ), idleSpritesDown },

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
        SetImageSequence(animation, direction);
        renderer.sprite = currentImageSequence[currentSpriteIndex];
        if (direction != currentDirection || animation != currentAnimationType)
            StartCycle();

        currentDirection = direction;
        currentAnimationType = animation;
    }

    void StartCycle()
    {
        print("start");
        if (currentAnimation != null)
            StopCycleImmediatly();
        animationRunning = true;
        currentAnimation = StartCoroutine(Animation(interval));
    }

    void StopCycle() => animationRunning = false;

    void StopCycleImmediatly()
    {
        StopCycle();
        currentSpriteIndex = 0;
        StopCoroutine(currentAnimation);
        currentAnimation = null;
    }

    private void UpdateSpriteIndex() => currentSpriteIndex = currentSpriteIndex % currentImageSequence.Length;

    private void IncrementSpriteIndex()
    {
        currentSpriteIndex ++;
        UpdateSpriteIndex();
    }

    private void SetImageSequence(AnimationType animation, Direction direction)
    {
        currentImageSequence = animationSpriteMap[new Tuple<AnimationType, Direction>(animation, direction)];
        UpdateSpriteIndex();
    }

    IEnumerator Animation(float interval)
    {
        while (true)
        {
            if (!animationRunning && currentSpriteIndex % 2 == 0)
                StopCycleImmediatly();

            yield return new WaitForSeconds(interval);
            IncrementSpriteIndex();
        }
    }
}
