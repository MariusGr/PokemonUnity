using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] CharacterControllerBase controller;
    [SerializeField] CharacterMovement movement;
    [SerializeField] CharacterAnimator animator;

    public CharacterControllerBase Controller => controller;
    public CharacterMovement Movement => movement;
    public CharacterAnimator Animator => animator;

    void Update()
    {

    }
}
