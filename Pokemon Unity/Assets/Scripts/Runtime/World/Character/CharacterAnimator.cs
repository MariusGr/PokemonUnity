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

    private Dictionary<Tuple<AnimationType, Direction>, Sprite[]> animationSpriteMap;
    private Sprite[] currentImageSequence;
    private int currentSpriteIndex = 0;
    private AnimationType currentAnimation = AnimationType.None;
    private Direction currentDirection = Direction.Down;
    private float lastTickTime = 0;
    private float intervalClock = 0;
    private Coroutine tickCoroutine;

    private void Awake()
    {
        animationSpriteMap = new()
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

        currentImageSequence = walkSpritesDown;
        lastTickTime = Time.time;
    }

    public void Set(AnimationType animation, Direction direction)
    {
        if (animation != currentAnimation || direction != currentDirection)
            SetImageSequence(animation, direction);

        if (direction != currentDirection)
            print(direction);

        currentDirection = direction;
        currentAnimation = animation;
    }

    public void Tick()
    {
        float deltaTime = Time.time - lastTickTime;

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

    IEnumerator TickCoroutine()
    {
        while(true)
        {
            Tick();
            yield return new WaitForEndOfFrame();
        }
    }

    public void StartAnimation()
    {
        if (tickCoroutine is not null)
            return;
        tickCoroutine = StartCoroutine(TickCoroutine());
    }

    public void StopAnimation()
    {
        if (tickCoroutine is null)
            return;
        StopCoroutine(tickCoroutine);
        tickCoroutine = null;
    }

    private void UpdateSpriteIndex()
    {
        currentSpriteIndex %= currentImageSequence.Length;
        renderer.sprite = currentImageSequence[currentSpriteIndex];
    }

    private void IncrementSpriteIndex()
    {
        currentSpriteIndex ++;
        UpdateSpriteIndex();
    }

    private void SetImageSequence(AnimationType animation, Direction direction)
    {
        if (direction != Direction.None)
            currentImageSequence = animationSpriteMap[new Tuple<AnimationType, Direction>(animation, direction)];
        UpdateSpriteIndex();
    }

    public Coroutine PlayExclaimBubbleAnimation()
        => StartCoroutine(ExclaimAnimationCoroutine());

    private IEnumerator ExclaimAnimationCoroutine()
    {
        exclaimBubble.SetActive(true);
        yield return new WaitForSeconds(1f);
        exclaimBubble.SetActive(false);
    }
}
