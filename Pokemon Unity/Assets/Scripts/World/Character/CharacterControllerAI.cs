using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable
{
    [SerializeField] public NPCData npcData;
    [SerializeField] public bool wantsToBattle;
    [SerializeField] public float challengeVisionDistance = 5f;

    override public CharacterData CharacterData => npcData;

    private void Awake()
    {
        if (wantsToBattle && !npcData.IsDefeated())
            EventManager.Instance.CheckNPCVisionEvent += CheckChallengeVision;    
    }

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
                Challenge(player);
            }
            else
            {
                Services.Get<IDialogBox>().DrawText(npcData.defaultText, DialogBoxContinueMode.User, true);
            }
        }
    }

    private void Challenge(Character player) => StartCoroutine(ChallengeCoroutine(player));

    private IEnumerator ChallengeCoroutine(Character player)
    {
        EventManager.Pause();
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
        EventManager.Instance.CheckNPCVisionEvent -= CheckChallengeVision;

        return true;
    }

    public void CheckChallengeVision()
    {
        RaycastHit hitInfo;
        if (character.RaycastForward(character.Movement.CurrentDirectionVector, LayerManager.Instance.PlayerLayerMask, out hitInfo, maxDistance: challengeVisionDistance -.2f))
            Challenge(hitInfo.collider.gameObject.GetComponent<Character>());
    }
}
