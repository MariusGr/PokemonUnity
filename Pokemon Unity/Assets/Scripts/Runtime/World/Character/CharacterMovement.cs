using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterMovement : Pausable
{
    private const float WalkingSpeed = 5f;

    enum Axis
    {
        None,
        Horizontal,
        Vertical,
    }

    [SerializeField] private Character character;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Direction currentDirection = Direction.Down;
    [SerializeField] private new BoxCollider collider;

    public float walkingSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float horizontalLast = 0;
    public float verticalLast = 0;
    private float horizontalChanged = 0;
    private float verticalChanged = 0;
    public bool Moving { get; private set; } = false;
    private bool lastMoving = false;
    private CharacterMovement following;

    private GridVector startDirection;
    private GridVector currentDirectionVector = GridVector.Down;
    public GridVector CurrentDirectionVector {
        get => currentDirectionVector;
        private set
        {
            currentDirectionVector = value.Normalized;
            currentDirection = currentDirectionVector.ToDirection();
        }
    }

    private Action<GridVector, AnimationType, float> onStartMovingOneTile;
    private Action onStopMoving;
    private readonly Queue<(GridVector position, AnimationType animation, float speed)> moveQueue = new();
    private bool movingThroughMoveQueue = false;
    private GridVector nextPosition;

    private void Awake() => nextPosition = character.Position;

    void Start()
    {
        controller.Move(Vector3.down * .1f);
        LookInDirection(currentDirection);
        startDirection = CurrentDirectionVector;
        SignUpForPause();

        if (following is not null)
            Follow(following);
    }

    public void Follow(CharacterMovement target)
    {
        collider.enabled = false;
        following = target;
        following.onStartMovingOneTile += AddToMoveQueue;
        following.onStopMoving += StopFollowMovement;
    }

    public void Unfollow()
    {
        if (following is null)
            return;
        collider.enabled = true;
        following.onStartMovingOneTile -= AddToMoveQueue;
        following.onStopMoving -= StopFollowMovement;
        following = null;
    }

    private void OnDestroy() => Unfollow();

    private void StopFollowMovement() => character.Animator.Set(AnimationType.None, (following.nextPosition - character.Position).ToDirection());

    protected override void Pause()
    {
        base.Pause();
        character.Animator.Set(AnimationType.None, currentDirection);
        StopAllCoroutines();
        Moving = false;
    }

    protected override void Unpause()
    {
        base.Unpause();
        Moving = false;
    }

    public void ProcessMovement(Direction direction, bool sprinting = false, bool checkPositionEvents = true, bool ignorePaused = false)
        => ProcessMovement(new GridVector(direction), sprinting, checkPositionEvents, ignorePaused);

    public void ProcessMovement(GridVector direction, bool sprinting = false, bool checkPositionEvents = true, bool ignorePaused = false)
        => ProcessMovement(direction.x, direction.y, sprinting, checkPositionEvents, ignorePaused);

    public void ProcessMovement(float horizontal, float vertical, bool sprinting, bool checkPositionEvents = true, bool ignorePaused = false)
    {
        if (!ignorePaused && paused)
            return;

        float currentSpeed;
        AnimationType animation;
        if (sprinting)
        {
            currentSpeed = sprintingSpeed;
            animation = AnimationType.Sprint;
        }
        else
        {
            currentSpeed = walkingSpeed;
            animation = AnimationType.Walk;
        }

        if (Moving)
            return;

        bool horizontalActive = horizontal != 0;
        bool verticalActive = vertical != 0;
        bool moveHorizontal = false;
        bool moveVertical = false;

        if (horizontal != horizontalLast)
            horizontalChanged = Time.time;
        if (vertical != verticalLast)
            verticalChanged = Time.time;

        horizontalLast = horizontal;
        verticalLast = vertical;

        if (verticalChanged > horizontalChanged && verticalActive)
            moveVertical = true;
        else if (horizontalActive)
            moveHorizontal = true;
        else
        {
            moveHorizontal = horizontalActive;
            moveVertical = verticalActive;
        }

        bool success = false;
        Vector3 direction = moveVertical ? Vector3.forward * Mathf.Sign(vertical) : moveHorizontal ? Vector3.right * Mathf.Sign(horizontal) : Vector3.zero;
        if (moveVertical || moveHorizontal)
            success = MoveInDirection(direction, animation, currentSpeed, checkPositionEvents);

        // TODO: Always gets triggered twice
        if (!success && lastMoving)
            StopMoving();

        lastMoving = Moving;
    }

    private void StopMoving()
    {
        character.Animator.Set(AnimationType.None, currentDirection);
        onStopMoving?.Invoke();
    }

    public void LookInPlayerDirection() => LookAtTarget(PlayerCharacter.Instance);
    public void LookAtTarget(Character target) => LookAtTarget(target.Movement.nextPosition);
    public void LookAtTarget(GridVector target) => LookInDirection(GridVector.GetLookAt(character.Position, target));
    public void LookInDirection(GridVector direction)
    {
        if (CurrentDirectionVector.Equals(direction) || direction.magnitude == 0)
            return;
        CurrentDirectionVector = direction;
        character.Animator.Set(AnimationType.None, currentDirection);
    }

    bool MoveInDirection(Vector3 direction, AnimationType animation, float speed, bool checkPositionEvents)
    {
        CurrentDirectionVector = new GridVector(direction);

        if (IsMovable(direction))
        {
            if (!Moving)
                StartMovingTo(new GridVector(transform.position + direction), animation, speed, checkPositionEvents);
            return true;
        }
        return false;
    }

    Coroutine MoveToPosition((GridVector position, AnimationType animation, float speed) data)
        => MoveToPosition(data.position, data.animation, data.speed);
    Coroutine MoveToPosition(GridVector position, AnimationType animation, float speed)
    {
        CurrentDirectionVector = new GridVector(position - transform.position);
        character.Animator.Set(animation, currentDirection);
        return StartCoroutine(MoveToCoroutine(position, speed, false));
    }

    public void LookInDirection(Direction direction) => LookInDirection(new GridVector(direction));
    public void LookInStartDirection() => LookInDirection(startDirection);

    void AddToMoveQueue(GridVector position, AnimationType animation = AnimationType.Walk, float speed = WalkingSpeed)
    {
        moveQueue.Enqueue((position, animation, speed));
        if (movingThroughMoveQueue)
            return;
        movingThroughMoveQueue = true;
        StartCoroutine(MoveThroughMoveQueueCoroutine());
    }

    private IEnumerator MoveThroughMoveQueueCoroutine()
    {
        while (moveQueue.Count > 0)
            yield return MoveToPosition(moveQueue.Dequeue());
        movingThroughMoveQueue = false;
    }

    private Coroutine StartMovingTo(GridVector position, AnimationType animation, float speed, bool checkPositionEvents)
    {
        Moving = true;
        CurrentDirectionVector = new GridVector(position - transform.position);
        character.Animator.Set(animation, currentDirection);
        onStartMovingOneTile?.Invoke(new GridVector(transform.position), animation, speed);
        return StartCoroutine(MoveToCoroutine(position, speed, checkPositionEvents));
    }

    IEnumerator MoveToCoroutine(GridVector target, float speed, bool checkPositionEvents)
    {
        GridVector start = new(transform.position);
        Vector3 up = Vector3.up * (-10);
        Vector3 direction = target - start;
        character.Animator.StartAnimation();
        nextPosition = target;

        while (!new GridVector(transform.position, start).Equals(target))
        {
            yield return new WaitForEndOfFrame();
            controller.Move((direction * speed + up) * Time.deltaTime);
        }

        transform.position = target + Vector3.up * transform.position.y;

        if (checkPositionEvents)
        {
            bool playerHasBeenDefeated = false;
            yield return PokemonManager.Instance.HandleWalkDamage(() => playerHasBeenDefeated = true);
            if (playerHasBeenDefeated)
                yield break;

            EncounterArea.CheckPositionRelatedEvents();
        }

        Moving = false;
        character.Animator.StopAnimation();
    }

    bool IsMovable(Vector3 direction)
        => !character.RaycastForward(direction, layerMask: LayerManager.Instance.MovementBlockingLayerMask);
}
