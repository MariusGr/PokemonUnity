using System.Collections;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable
{
    public NpcCharacter NpcCharacter => (NpcCharacter)character;

    public void Interact(Character player)
    {
        if (StoryEvent.EventHappening)
            character.Movement.LookInPlayerDirection();
        else if (NpcCharacter.NpcData.wantsToBattle && NpcCharacter.NpcData.hasBeenDefeated)
        {
            character.Movement.LookInPlayerDirection();
            DialogBox.Instance.DrawText(NpcCharacter.NpcData.afterDefeatText, DialogBoxContinueMode.User, true);
        }
        else
        {
            if (NpcCharacter.NpcData.wantsToBattle)
            {
                Challenge(player);
            }
            else
            {
                character.Movement.LookInPlayerDirection();
                DialogBox.Instance.DrawText(NpcCharacter.NpcData.GetDialogText(), DialogBoxContinueMode.User, true);
            }
        }
    }

    private void Challenge(Character player)
    {
        print("Challenge");
        EventManager.Pause();
        BgmHandler.Instance.PlayOverlay(NpcCharacter.NpcData.challengeMusicTrack);
        StartCoroutine(ChallengeCoroutine(player));
    }

    private IEnumerator ChallengeCoroutine(Character player)
    {
        GridVector direction = GridVector.GetLookAt(character.Position, player.Position);
        character.Movement.LookInDirection(direction);

        yield return character.Animator.PlayExclaimBubbleAnimation();

        yield return MoveToPosition(player.Position, direction, 1);

        player.Movement.LookInDirection(-direction);
        yield return DialogBox.Instance.DrawText(NpcCharacter.NpcData.challengeText, DialogBoxContinueMode.User, true);
        BattleManager.Instance.StartNewBattle(player.Data, NpcCharacter.NpcData, BattleEndReaction);
    }

    public void BattleEndReaction(bool npcDefeated)
    {
        if (npcDefeated)
        {
            NpcCharacter.Defeat();
            DialogBox.Instance.DrawText(NpcCharacter.NpcData.defeatedText, DialogBoxContinueMode.User, true);
        }
        else
            transform.position = character.StartPosition;

        EventManager.Unpause();
    }

    public bool CheckChallengeVision()
    {
        RaycastHit hitInfo;
        if (character.RaycastForward(
            character.Movement.CurrentDirectionVector,
            LayerManager.Instance.VisionBlockingForAILayerMask,
            out hitInfo,
            maxDistance: NpcCharacter.NpcData.challengeVisionDistance - .2f)
        )
        {
            Character otherCharacter = hitInfo.collider.gameObject.GetComponent<Character>();
            if (otherCharacter is null || !otherCharacter.IsPlayer)
                return false;

            Challenge(otherCharacter);
            return true;
        }

        return false;
    }

    public Coroutine MoveToPosition(GridVector target, GridVector direction, int keepDistance = 0, bool checkPositionEvents = false)
        => StartCoroutine(MoveToPositionCoroutine(target, direction, keepDistance, checkPositionEvents));

    private IEnumerator MoveToPositionCoroutine(GridVector target, GridVector direction, int keepDistance = 0, bool checkPositionEvents = false)
    {
        character.Movement.LookInDirection(direction);
        target -= keepDistance * direction;
        while (!new GridVector(transform.position, character.StartPosition).Equals(target))
        {
            yield return new WaitForEndOfFrame();
            character.Movement.ProcessMovement(direction, checkPositionEvents: checkPositionEvents, ignorePaused: true);
        }
    }
}
