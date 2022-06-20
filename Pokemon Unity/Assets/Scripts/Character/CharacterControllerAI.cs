using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable
{
    [SerializeField] public NPCData npcData;

    override public CharacterData CharacterData => npcData;

    public void Interact(Character player)
    {
        //Services.Get<IDialogBox>().DrçawText(new string[] { "hi!!!!" });
        GridVector direction = player.position - character.position;
        character.Movement.LookInDirection(direction);

        Services.Get<IBattleManager>().StartNewBattle(player.characterData, npcData);
    }
}
