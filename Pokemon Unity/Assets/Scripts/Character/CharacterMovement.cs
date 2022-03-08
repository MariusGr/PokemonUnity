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

    [SerializeField] Character character;
    [SerializeField] CharacterController controller;
    public float walkingSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float currentSpeed;
    public float horizontalLast = 0;
    public float verticalLast = 0;
    float horizontalChanged = 0;
    float verticalChanged = 0;
    bool moving = false;
    Axis lastChangedAxis = Axis.None;
    GridVector currentDirection = GridVector.Down;

    void Start()
    {
        currentSpeed = walkingSpeed;
    }

    void Update()
    {
        
    }

    public void ProcessMovement(float horizontal, float vertical, bool sprinting)
    {
        if (sprinting)
            currentSpeed = sprintingSpeed;
        else
            currentSpeed = walkingSpeed;

        if (moving)
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

        if (moveHorizontal)
            Move(Vector3.right * Mathf.Sign(horizontal));
        else if (moveVertical)
            Move(Vector3.forward * Mathf.Sign(vertical));
        else
            character.Animator.Refresh(AnimationType.None, currentDirection.ToDirection());
    }

    void Move(Vector3 direction)
    {
        currentDirection = new GridVector(direction);
        Direction d = currentDirection.ToDirection();

        if (IsMovable(direction))
        {
            StartCoroutine(MoveTo(new GridVector(transform.position + direction)));
            character.Animator.Refresh(AnimationType.Walk, d);
        }
        else
        {
            print("blocked");
            character.Animator.Refresh(AnimationType.None, d);
        }
    }

    IEnumerator MoveTo(GridVector target)
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

        yield return null;
    }

    bool IsMovable(Vector3 direction)
    {
        return !Physics.Raycast(origin: transform.position + Vector3.up * .5f, direction: direction, maxDistance: .8f, layerMask: LayerManager.Instance.MovementBlockingLayerMask);
    }
}
