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

    [SerializeField] CharacterController characterController;
    public float walkingSpeed = 3f;
    public float sprintingSpeed = 6f;
    public float currentSpeed;
    bool moving = false;
    Axis lastChangedAxis = Axis.None;

    void Start()
    {
        currentSpeed = walkingSpeed;
    }

    void Update()
    {
        
    }

    public void ProcessMovement(float horizontal, float vertical, bool horizontalChanged, bool verticalChanged, bool sprinting)
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

        if (verticalChanged && verticalActive)
            moveVertical = true;
        else if (horizontalChanged && horizontalActive)
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
    }

    void Move(Vector3 direction)
    {
        if (IsMovable(direction))
            StartCoroutine(MoveTo(new GridVector(transform.position + direction)));
        else
            print("blocked");
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
            characterController.Move((direction * currentSpeed + up) * Time.deltaTime);
        }

        transform.position = (Vector3)target + Vector3.up * transform.position.y;
        moving = false;

        yield return null;
    }

    bool IsMovable(Vector3 direction)
    {
        return !Physics.Raycast(origin: transform.position + Vector3.up * .5f, direction: direction, maxDistance: .8f, layerMask: LayerManager.Instance.MovementBlockingLayerMask);
    }
}
