using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] CharacterControllerBase controller;
    [SerializeField] CharacterMovement movement;

    public CharacterMovement Movement => movement;

    void Update()
    {

    }
}
