using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable
{
    [SerializeField] public NPCData npcData;

    override public CharacterData CharacterData => npcData;

    public void Interact(Character player)
    {
        if (npcData.hasBeenDefeated)
        {
            Services.Get<IDialogBox>().DrawText(npcData.afterDefeatText, DialogBoxContinueMode.User, true);
        }
        else
        {
            GridVector direction = player.position - character.position;
            character.Movement.LookInDirection(direction);

            Services.Get<IBattleManager>().StartNewBattle(player.characterData, npcData, BattleEndReaction);
        }
    }

    public bool BattleEndReaction(bool npcDefeated)
    {
        Services.Get<IBattleManager>().EndBattle();

        if (npcDefeated)
            Services.Get<IDialogBox>().DrawText(npcData.defeatedText, DialogBoxContinueMode.User, true);
        else
            Services.Get<IDialogBox>().Close();

        character.Reset();

        return true;
    }
}
