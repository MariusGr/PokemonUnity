using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class NpcCharacter : Character
{
    private static readonly List<NpcCharacter> battlingNPCs = new();

    public static bool CheckAllNPCVision()
    {
        foreach (var npcCharacter in battlingNPCs)
            if (npcCharacter.AiController.CheckChallengeVision())
                return true;
        return false;
    }

    [field: SerializeField] public NpcData NpcData { get; private set; }
    public override CharacterData Data => NpcData;
    public CharacterControllerAI AiController => (CharacterControllerAI)Controller;

    public void Defeat()
    {
        battlingNPCs.Remove(this);
        NpcData.hasBeenDefeated = true;
    }

    public override string GetKey()
    {
        GridVector startPosition = new(StartPosition);
        return $"{GetType()}_{startPosition.x}_{startPosition.y}";
    }

    public bool WillChallengePlayer => NpcData.wantsToBattle && !NpcData.IsDefeated() && !NpcData.hasBeenDefeated;

    public override JSONNode ToJSON()
    {
        JSONNode json = base.ToJSON();
        json.Add("hasBeenDefeated", NpcData.hasBeenDefeated);
        return json;
    }

    public override void LoadFromJSON(JSONObject json)
    {
        JSONNode jsonData = json[GetKey()];
        NpcData.hasBeenDefeated = jsonData["hasBeenDefeated"];
        if (WillChallengePlayer)
            battlingNPCs.Add(this);
        base.LoadFromJSON(json);
        PokemonsLoadDefault();
    }

    public override void LoadDefault()
    {
        base.LoadDefault();
        if (WillChallengePlayer)
            battlingNPCs.Add(this);
    }
}
