using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, Interactable
{
    [SerializeField] string[] text;

    public void Interact(Character player)
    {
        DialogBox.Instance.DrawText(text);
        GridVector direction = player.position - character.position;
        character.Movement.LookInDirection(direction);
    }
}
