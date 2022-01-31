using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    public float walkingSpeed = 3f;
    public float currentSpeed;
    bool moving = false;

    void Start()
    {
        currentSpeed = walkingSpeed;
    }

    void Update()
    {
        
    }

    public void ProcessMovement(float horizontal, float vertical, bool sprinting)
    {
        if (moving)
            return;

        bool horizontalActive = horizontal != 0;
        bool verticalActive = vertical != 0;

        if (horizontalActive || verticalActive)
            if (horizontalActive)
                Move(Vector3.right * Mathf.Sign(horizontal));
            else if (verticalActive)
                Move(Vector3.forward * Mathf.Sign(vertical));
    }

    void Move(Vector3 direction)
    {
        StartCoroutine(MoveTo(new GridVector(transform.position + direction)));
    }

    IEnumerator MoveTo(GridVector target)
    {
        moving = true;
        GridVector start = new GridVector(transform.position);
        Vector3 up = Vector3.up * (-10);
        Vector3 direction = target - start;

        while (!new GridVector(transform.position, start).Equals(target))
        {
            characterController.Move((direction * currentSpeed + up) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        transform.position = (Vector3)target + Vector3.up * transform.position.y;
        moving = false;

        yield return null;
    }
}
