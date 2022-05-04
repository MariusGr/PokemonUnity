using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, Interactable
{
    [SerializeField] public NPCData npcData;

    void Awake()
    {
        base.characterData = npcData;
    }

    public void Interact(Character player)
    {
        //Services.Get<IDialogBox>().DrawText(new string[] { "hi!!!!" });
        GridVector direction = player.position - character.position;
        character.Movement.LookInDirection(direction);

        Services.Get<IBattleManager>().StartNewBattle(player.characterData, npcData);
    }
}