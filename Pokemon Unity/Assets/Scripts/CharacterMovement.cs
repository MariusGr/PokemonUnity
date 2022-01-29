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
                StartCoroutine(Move(Vector3.right * Mathf.Sign(horizontal)));
            else if (verticalActive)
                StartCoroutine(Move(Vector3.forward * Mathf.Sign(vertical)));
    }

    IEnumerator Move(Vector3 direction)
    {
        moving = true;
        float alpha = 0;
        Vector3 targetPositon = transform.position + direction;
        direction += -Vector3.up;
        while (Mathf.Abs(transform.position.x - Mathf.Round(targetPositon.x)) > 0.01f
            && Mathf.Abs(transform.position.z - Mathf.Round(targetPositon.z)) > 0.01f)
        {
            alpha += Time.deltaTime * currentSpeed;
            characterController.Move(direction * currentSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        //transform.position = targetPositon;
        moving = false;

        yield return 0;
    }
}
