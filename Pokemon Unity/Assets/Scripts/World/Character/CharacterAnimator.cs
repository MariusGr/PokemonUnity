using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    None,
    Walk,
    Sprint,
}

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] new SpriteRenderer renderer;

    [SerializeField] GameObject exclaimBubble;

    [SerializeField] Sprite[] idleSpritesRight;
    [SerializeField] Sprite[] idleSpritesLeft;
    [SerializeField] Sprite[] idleSpritesUp;
    [SerializeField] Sprite[] idleSpritesDown;

    [SerializeField] Sprite[] walkSpritesRight;
    [SerializeField] Sprite[] walkSpritesLeft;
    [SerializeField] Sprite[] walkSpritesUp;
    [SerializeField] Sprite[] walkSpritesDown;

    [SerializeField] Sprite[] sprintSpritesRight;
    [SerializeField] Sprite[] sprintSpritesLeft;
    [SerializeField] Sprite[] sprintSpritesUp;
    [SerializeField] Sprite[] sprintSpritesDown;

    [SerializeField] float interval;

    Dictionary<Tuple<AnimationType, Direction>, Sprite[]> animationSpriteMap;
    Dictionary<AnimationType, float> animationTypeToInterval;
    Sprite[] currentImageSequence;
    int currentSpriteIndex = 0;
    AnimationType currentAnimation = AnimationType.None;
    Direction currentDirection = Direction.Down;
    float lastTickTime = 0;
    float intervalClock = 0;

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

            { new Tuple<AnimationType, Direction>(
                AnimationType.Sprint, Direction.Right
            ), sprintSpritesRight },
            { new Tuple<AnimationType, Direction>(
                AnimationType.Sprint, Direction.Left
            ), sprintSpritesLeft },
            { new Tuple<AnimationType, Direction>(
                AnimationType.Sprint, Direction.Up
            ), sprintSpritesUp },
            { new Tuple<AnimationType, Direction>(
                AnimationType.Sprint, Direction.Down
            ), sprintSpritesDown },
        };
        animationTypeToInterval = new Dictionary<AnimationType, float>()
        {
            { AnimationType.Walk, .1f },
        };

        currentImageSequence = walkSpritesDown;
        lastTickTime = Time.time;
    }

    public void Tick()
        => Tick(currentAnimation, currentDirection);

    public void Tick(AnimationType animation, Direction direction)
    {
        float deltaTime = Time.time - lastTickTime;
        
        if (animation != currentAnimation || direction != currentDirection)
            SetImageSequence(animation, direction);
        
        currentDirection = direction;
        currentAnimation = animation;

        if (currentAnimation != AnimationType.None)
        {
            intervalClock += deltaTime;
            if (intervalClock >= interval)
            {
                intervalClock = 0;
                IncrementSpriteIndex();
            }
        }
        else
            intervalClock = 0;

        lastTickTime = Time.time;
    }

    private void UpdateSpriteIndex()
    {
        currentSpriteIndex = currentSpriteIndex % currentImageSequence.Length;
        renderer.sprite = currentImageSequence[currentSpriteIndex];
    }

    private void IncrementSpriteIndex()
    {
        currentSpriteIndex ++;
        UpdateSpriteIndex();
    }

    private void SetImageSequence(AnimationType animation, Direction direction)
    {
        print(direction);
        if (direction != Direction.None)
            currentImageSequence = animationSpriteMap[new Tuple<AnimationType, Direction>(animation, direction)];
        intervalClock = 0;
        currentSpriteIndex = (animation == AnimationType.None) ? 0 : 1;
        UpdateSpriteIndex();
    }

    public Coroutine PlayExclaimBubbleAnimation()
    {
        return StartCoroutine(ExclaimAnimationCoroutine());
    }

    private IEnumerator ExclaimAnimationCoroutine()
    {
        exclaimBubble.SetActive(true);
        yield return new WaitForSeconds(1f);
        exclaimBubble.SetActive(false);
    }
}
