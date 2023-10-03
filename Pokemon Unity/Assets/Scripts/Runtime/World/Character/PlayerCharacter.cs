using System.Collections;
using UnityEngine;
using SimpleJSON;

public class PlayerCharacter : Character
{
    public static PlayerCharacter Instance { get; private set; }

    [SerializeField] private StoryEvent caughtAllPokemonsStoryEvent;

    public override bool IsPlayer => true;

    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    public override CharacterData Data => PlayerData;
    private CharacterControllerPlayer PlayerController => (CharacterControllerPlayer)Controller;

    public PlayerCharacter() => Instance = this;

    public void OnCaughtAllPokemons() => caughtAllPokemonsStoryEvent.TryInvoke();

    public Coroutine Defeat() => StartCoroutine(DefeatCoroutine());
    private IEnumerator DefeatCoroutine()
    {
        yield return DialogBox.Instance.DrawText($"Tja... ab ins Poke-Center. Schei?e gelaufen...", DialogBoxContinueMode.User, closeAfterFinish: true);
        PlayerData.Instance.HealAllPokemons();
        transform.position = PlayerData.lastPokeCenterEntrance.position;
        Movement.LookInDirection(Direction.Down);
    }

    public void EnterEntranceTreshhold(Door entrance) => PlayerController.EnterEntranceTrehshold(entrance);
    public void LeaveEntranceTrehshold(Door entrance) => PlayerController.LeaveEntranceTrehshold(entrance);
    public void TravelToEntrance(Door entrance) => Movement.Teleport(entrance.spawnPosition, -new GridVector(entrance.directionTriggerToEntrance));

    public override string GetKey() => $"{GetType()}";

    public override JSONNode ToJSON()
    {
        JSONNode json = base.ToJSON();

        json.Add("money", PlayerData.money);
        json.Add("position", transform.position);
        json.Add("direction", (Vector3)Movement.CurrentDirectionVector);
        json.Add("items", PlayerData.ItemsToJSON());
        json.Add("pokemons", PlayerData.PokemonsToJSON());
        json.Add("pokemonsInBox", PlayerData.PokemonsInBoxToJSON());
        json.Add("pokemonsSeen", PlayerData.SeenPokemonsToJSON());
        // TODO: save last pokemon center
        //json.Add("lastPokeCenterEntrance", );

        return json;
    }

    public override void LoadFromJSON(JSONObject json)
    {
        JSONNode jsonData = json[GetKey()];
        PlayerData.money = jsonData["money"];
        transform.position = jsonData["position"];
        Movement.LookInDirection(new GridVector(jsonData["direction"]));
        PlayerData.LoadItemsFromJSON((JSONArray)jsonData["items"]);
        PlayerData.LoadPokemonsFromJSON((JSONArray)jsonData["pokemons"]);
        PlayerData.LoadPokemonsInBoxFromJSON((JSONArray)jsonData["pokemonsInBox"]);
        PlayerData.LoadSeenPokemonsFromJSON((JSONArray)jsonData["pokemonsSeen"]);
        base.LoadFromJSON(json);
    }

    public override void LoadDefault()
    {
        PlayerData.LoadDefault();
        base.LoadDefault();
    }
}
