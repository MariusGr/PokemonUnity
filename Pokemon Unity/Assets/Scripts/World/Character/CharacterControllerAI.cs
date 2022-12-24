using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerAI : CharacterControllerBase, IInteractable, ISavable
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
    [SerializeField] private AudioClip challengeMusicTrack;

    public bool willChallengePlayer => wantsToBattle && !npcData.IsDefeated() && !npcData.hasBeenDefeated;
    override public CharacterData CharacterData => npcData;

    override public void Initialize()
    {
        Services.Get<ISaveGameManager>().Register(this);
    }

    public void Interact(Character player)
    {
        if (wantsToBattle && npcData.hasBeenDefeated)
        {
            character.Movement.LookInPlayerDirection();
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
                character.Movement.LookInPlayerDirection();
                Services.Get<IDialogBox>().DrawText(npcData.defaultText, DialogBoxContinueMode.User, true);
            }
        }
    }

    private void Challenge(Character player)
    {
        print("Challenge");
        EventManager.Pause();
        BgmHandler.Instance.PlayOverlay(challengeMusicTrack);
        StartCoroutine(ChallengeCoroutine(player));
    }

    private IEnumerator ChallengeCoroutine(Character player)
    {
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
        battlingNPCs.Remove(this);

        if (npcDefeated)
            Services.Get<IDialogBox>().DrawText(npcData.defeatedText, DialogBoxContinueMode.User, true);

        EventManager.Unpause();
        return true;
    }

    public bool CheckChallengeVision()
    {
        RaycastHit hitInfo;
        if (character.RaycastForward(
            character.Movement.CurrentDirectionVector,
            LayerManager.Instance.VisionBlockingForAILayerMask,
            out hitInfo,
            maxDistance: challengeVisionDistance - .2f)
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
        while (!new GridVector(transform.position, character.startPosition).Equals(target))
        {
            yield return new WaitForEndOfFrame();
            character.Movement.ProcessMovement(direction, checkPositionEvents: checkPositionEvents, ignorePaused: true);
        }
    }

    public string GetKey()
    {
        GridVector startPosition = new GridVector(character.startPosition);
        return $"{GetType()}_{startPosition.x}_{startPosition.y}";
    }

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();
        json.Add("hasBeenDefeated", npcData.hasBeenDefeated);
        return json;
    }

    public void LoadFromJSON(JSONObject json)
    {
        JSONNode jsonData = json[GetKey()];
        npcData.hasBeenDefeated = jsonData["hasBeenDefeated"];
        character.LoadDefault();
        if (willChallengePlayer)
            battlingNPCs.Add(this);
    }

    public void LoadDefault()
    {
        character.LoadDefault();
        if (willChallengePlayer)
            battlingNPCs.Add(this);
    }
}
