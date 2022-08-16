using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerPlayer : CharacterControllerBase
{
    bool paused = false;

    public PlayerData characterData;
    override public CharacterData CharacterData => characterData;

    void Awake()
    {
        EventManager.Instance.PauseEvent += Pause;
        EventManager.Instance.UnpauseEvent += Unpause;
    }

    void Update()
    {
        if (!paused)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            sprinting = Input.GetButton("Sprint");

            if (Input.GetButtonDown("Submit"))
                character.TryInteract();
        }
        else
        {
            horizontal = 0;
            vertical = 0;
            sprinting = false;
        }

        character.Movement.ProcessMovement(horizontal, vertical, sprinting);
    }

    private void Pause()
    {
        paused = true;
        print("pause");
    }

    private void Unpause()
    {
        paused = false;
    }
}
