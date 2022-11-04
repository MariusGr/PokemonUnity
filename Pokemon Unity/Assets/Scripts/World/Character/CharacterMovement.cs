using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    enum Axis
    {
        None,
        Horizontal,
        Vertical,
    }

    [SerializeField] private Character character;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Direction currentDirection = Direction.Down;
    public float walkingSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float currentSpeed;
    public float horizontalLast = 0;
    public float verticalLast = 0;
    private float horizontalChanged = 0;
    private float verticalChanged = 0;
    private bool moving = false;

    private GridVector startDirection;
    private GridVector currentDirectionVector = GridVector.Down;
    public GridVector CurrentDirectionVector {
        get => currentDirectionVector;
        private set
        {
            currentDirectionVector = value;
            currentDirection = currentDirectionVector.ToDirection();
        }
    }

    void Start()
    {
        currentSpeed = walkingSpeed;
        controller.Move(Vector3.down * .1f);
        LookInDirection(currentDirection);
        startDirection = CurrentDirectionVector;
    }

    public void ProcessMovement(Direction direction, bool sprinting = false, bool checkPositionEvents = true)
        => ProcessMovement(new GridVector(direction), sprinting, checkPositionEvents);

    public void ProcessMovement(GridVector direction, bool sprinting = false, bool checkPositionEvents = true)
        => ProcessMovement(direction.x, direction.y, sprinting, checkPositionEvents);

    public void ProcessMovement(float horizontal, float vertical, bool sprinting, bool checkPositionEvents = true)
    {
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

        if (moving)
        {
            character.Animator.Tick();
            return;
        }

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

        if (moveHorizontal)
            Move(Vector3.right * Mathf.Sign(horizontal), animation, checkPositionEvents);
        else if (moveVertical)
            Move(Vector3.forward * Mathf.Sign(vertical), animation, checkPositionEvents);
        else
            character.Animator.Tick(AnimationType.None, currentDirection);
    }

    void Move(Vector3 direction, AnimationType animation, bool checkPositionEvents)
    {
        CurrentDirectionVector = new GridVector(direction);

        if (IsMovable(direction))
        {
            StartCoroutine(MoveTo(new GridVector(transform.position + direction), checkPositionEvents));
            character.Animator.Tick(animation, currentDirection);
        }
        else
            character.Animator.Tick(AnimationType.None, currentDirection);
    }

    public void LookInPlayerDirection() => LookInDirection(GridVector.GetLookAt(character.position, Character.PlayerCharacter.position));
    public void LookInDirection(GridVector direction)
    {
        CurrentDirectionVector = direction;
        character.Animator.Tick(AnimationType.None, currentDirection);
    }

    public void LookInDirection(Direction direction) => LookInDirection(new GridVector(direction));
    public void LookInStartDirection() => LookInDirection(startDirection);

    IEnumerator MoveTo(GridVector target, bool checkPositionEvents)
    {
        moving = true;
        GridVector start = new GridVector(transform.position);
        Vector3 up = Vector3.up * (-10);
        Vector3 direction = target - start;

        while (!new GridVector(transform.position, start).Equals(target))
        {
            yield return new WaitForEndOfFrame();
            controller.Move((direction * currentSpeed + up) * Time.deltaTime);
        }

        transform.position = target + Vector3.up * transform.position.y;
        moving = false;

        if (checkPositionEvents)
            EncounterArea.CheckPositionRelatedEvents();
    }

    bool IsMovable(Vector3 direction)
        => !character.RaycastForward(direction, layerMask: LayerManager.Instance.MovementBlockingLayerMask);
}
