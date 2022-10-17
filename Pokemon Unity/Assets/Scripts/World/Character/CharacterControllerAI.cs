using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable
{
    private static List<CharacterControllerAI> battlingNPCs = new List<CharacterControllerAI>();

    public static bool CheckAllNPCVision()
    {
        foreach (CharacterControllerAI c in battlingNPCs)
            if (c.CheckChallengeVision())
                return true;
        return false;
    }

    [SerializeField] public NPCData npcData;
    [SerializeField] public bool wantsToBattle;
    [SerializeField] public float challengeVisionDistance = 5f;

    public bool willChallengePlayer => wantsToBattle || !npcData.IsDefeated();
    override public CharacterData CharacterData => npcData;

    private bool isInBattle = false;

    override public void Init()
    {
        if (willChallengePlayer)
            battlingNPCs.Add(this);
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
        isInBattle = true;

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
            Services.Get<IDialogBox>().Close();
        }

        isInBattle = false;
        character.Reset();

        return true;
    }

    public bool CheckChallengeVision()
    {
        if (willChallengePlayer)
            return false;

        RaycastHit hitInfo;
        if (character.RaycastForward(character.Movement.CurrentDirectionVector, LayerManager.Instance.PlayerLayerMask, out hitInfo, maxDistance: challengeVisionDistance - .2f))
        {
            Challenge(hitInfo.collider.gameObject.GetComponent<Character>());
            return true;
        }

        return false;
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
