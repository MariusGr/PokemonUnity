using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable
{
    [SerializeField] public NPCData npcData;
    [SerializeField] public bool wantsToBattle;
    [SerializeField] public float challengeVisionDistance = 5f;

    override public CharacterData CharacterData => npcData;

    override public void Init()
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
        EventManager.Instance.CheckNPCVisionEvent -= CheckChallengeVision;
        EventManager.Pause();

        GridVector direction = GridVector.GetLookAt(character.position, player.position);
        character.Movement.LookInDirection(direction);

        yield return character.Animator.PlayExclaimBubbleAnimation();

        yield return MoveToPosition(player.position, direction, 1);

        player.Movement.LookInDirection(-direction);
        yield return Services.Get<IDialogBox>().DrawText(npcData.challengeText, DialogBoxContinueMode.User, true);
        Services.Get<IBattleManager>().StartNewBattle(player.characterData, npcData, BattleEndReaction);
    }

    public bool BattleEndReaction(bool npcDefeated)
    {
        Services.Get<IBattleManager>().EndBattle();

        if (npcDefeated)
            Services.Get<IDialogBox>().DrawText(npcData.defeatedText, DialogBoxContinueMode.User, true);
        else
        {
            EventManager.Instance.CheckNPCVisionEvent += CheckChallengeVision;
            Services.Get<IDialogBox>().Close();
        }

        character.Reset();

        return true;
    }

    public void CheckChallengeVision()
    {
        RaycastHit hitInfo;
        if (character.RaycastForward(character.Movement.CurrentDirectionVector, LayerManager.Instance.PlayerLayerMask, out hitInfo, maxDistance: challengeVisionDistance -.2f))
            Challenge(hitInfo.collider.gameObject.GetComponent<Character>());
    }

    public Coroutine MoveToPosition(GridVector target, GridVector direction, int keepDistance = 0)
        => StartCoroutine(MoveToPositionCoroutine(target, direction, keepDistance));

    private IEnumerator MoveToPositionCoroutine(GridVector target, GridVector direction, int keepDistance = 0)
    {
        character.Movement.LookInDirection(direction);
        target -= keepDistance * direction;
        while (!new GridVector(transform.position, character.startPosition).Equals(target))
        {
            yield return new WaitForEndOfFrame();
            character.Movement.ProcessMovement(direction);
        }
    }
}
