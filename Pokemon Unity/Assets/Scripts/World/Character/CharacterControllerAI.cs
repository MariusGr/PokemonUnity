using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable
{
    [SerializeField] public NPCData npcData;
    [SerializeField] public bool wantsToBattle;

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

            if (wantsToBattle)
            {
                StartCoroutine(Challenge(player));
            }
            else
            {
                Services.Get<IDialogBox>().DrawText(npcData.defaultText, DialogBoxContinueMode.User, true);
            }
        }
    }

    private IEnumerator Challenge(Character player)
    {
        yield return character.Animator.PlayExclaimBubbleAnimation();
        yield return Services.Get<IDialogBox>().DrawText(npcData.challengeText, DialogBoxContinueMode.User, true);
        Services.Get<IBattleManager>().StartNewBattle(player.characterData, npcData, BattleEndReaction);
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
