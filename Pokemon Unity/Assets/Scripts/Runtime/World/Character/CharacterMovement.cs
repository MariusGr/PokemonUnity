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
    [SerializeField] private float lookOnlySeconds = .2f;
    [SerializeField] private float fullStopDelaySeconds = .2f;
    [SerializeField] private new BoxCollider collider;

    public float walkingSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float horizontalLast = 0;
    public float verticalLast = 0;
    private float horizontalChanged = 0;
    private float verticalChanged = 0;
    public bool Moving { get; private set; } = false;
    private bool lastMoving = false;
    private float fullStopSpring;

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

    private readonly Queue<(GridVector position, AnimationType animation, float speed)> moveQueue = new();
    private bool movingThroughMoveQueue = false;
    private Coroutine moveQueueCoroutine;
    private GridVector nextPosition;
    private CharacterMovement follower;
    public bool Following { get; private set; } 

    private void Awake() => nextPosition = character.Position;

    void Start()
    {
        controller.Move(Vector3.down * .1f);
        LookInDirection(currentDirection);
        startDirection = CurrentDirectionVector;
        SignUpForPause();
    }

    public void AddFollower(CharacterMovement follower)
    {
        if (follower is null)
            return;
        this.follower = follower;
        follower.LookAtTarget(character.Position, true);
        follower.Following = true;
    }

    public void Unfollow()
    {
        if (follower is null)
            return;
        follower.collider.enabled = true;
        follower.Following = false;
        follower = null;
    }

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
        {
            fullStopSpring = Mathf.Min(1f, fullStopSpring + Time.deltaTime / fullStopDelaySeconds);
            return;
        }
        fullStopSpring = Mathf.Max(0, fullStopSpring - Time.deltaTime / fullStopDelaySeconds);

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

        bool lookOnly;
        if (verticalChanged > horizontalChanged && verticalActive)
        {
            moveVertical = true;
            lookOnly = WillOnlyLookDirectionChange(verticalChanged);
        }
        else if (horizontalActive)
        {
            moveHorizontal = true;
            lookOnly = WillOnlyLookDirectionChange(horizontalChanged);
        }
        else
        {
            moveHorizontal = horizontalActive;
            moveVertical = verticalActive;
            lookOnly = false;
        }

        bool success = false;
        Vector3 direction = moveVertical ? Vector3.forward * Mathf.Sign(vertical) : moveHorizontal ? Vector3.right * Mathf.Sign(horizontal) : Vector3.zero;
        if (moveVertical || moveHorizontal)
            if (lookOnly)
            {
                LookInDirection(direction, true);
                success = true;
            }
            else
                success = MoveInDirection(direction, animation, currentSpeed, checkPositionEvents);

        // TODO: Always gets triggered twice
        if (!success && lastMoving)
            StopMoving();

        lastMoving = Moving;
    }

    private bool WillOnlyLookDirectionChange(float timeSincePress)
        => fullStopSpring <= Mathf.Epsilon &&  Time.time - timeSincePress < lookOnlySeconds;

    private void StopMoving()
    {
        character.Animator.Set(AnimationType.None, currentDirection);
        follower?.LookInDirection((nextPosition - follower.character.Position).ToDirection(), forceUpdate: true);
    }

    public void Teleport(Vector3 position, Direction direction) => Teleport(position, new GridVector(direction));
    public void Teleport(Vector3 position, GridVector direction)
    {
        follower?.StopMoveQueue();
        transform.position = position;
        LookInDirection(direction);
        follower?.Teleport(position - new GridVector(direction), direction);
    }

    public void LookInPlayerDirection() => LookAtTarget(PlayerCharacter.Instance);
    public void LookAtTarget(Character target, bool forceUpdate = false) => LookAtTarget(target.Movement.nextPosition, forceUpdate);
    public void LookAtTarget(GridVector target, bool forceUpdate = false) => LookInDirection(GridVector.GetLookAt(character.Position, target), forceUpdate);
    public void LookInStartDirection() => LookInDirection(startDirection);
    public void LookInDirection(Vector3 direction, bool forceUpdate = false) => LookInDirection(new GridVector(direction), forceUpdate);
    public void LookInDirection(Direction direction, bool forceUpdate = false) => LookInDirection(new GridVector(direction), forceUpdate);
    public void LookInDirection(GridVector direction, bool forceUpdate = false)
    {
        if (!forceUpdate && (CurrentDirectionVector.Equals(direction) || direction.magnitude == 0))
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
        else
            character.Animator.Set(AnimationType.None, currentDirection);
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

    void AddToMoveQueue(GridVector position, AnimationType animation = AnimationType.Walk, float speed = WalkingSpeed)
    {
        moveQueue.Enqueue((position, animation, speed));
        if (movingThroughMoveQueue)
            return;
        movingThroughMoveQueue = true;
        moveQueueCoroutine = StartCoroutine(MoveThroughMoveQueueCoroutine());
    }

    public void StopMoveQueue()
    {
        if (moveQueueCoroutine is null)
            return;
        moveQueue.Clear();
        StopCoroutine(moveQueueCoroutine);
        movingThroughMoveQueue = false;
        moveQueueCoroutine = null;
    }

    private IEnumerator MoveThroughMoveQueueCoroutine()
    {
        while (moveQueue.Count > 0)
            yield return MoveToPosition(moveQueue.Dequeue());
        StopMoveQueue();
    }

    private Coroutine StartMovingTo(GridVector position, AnimationType animation, float speed, bool checkPositionEvents)
    {
        Moving = true;
        CurrentDirectionVector = new GridVector(position - transform.position);
        character.Animator.Set(animation, currentDirection);
        follower?.AddToMoveQueue(new GridVector(transform.position), animation, speed);
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
