using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class PlayerCharacter : Character, ISavable
{
    public static PlayerCharacter Instance { get; private set; }

    [SerializeField] private StoryEvent caughtAllPokemonsStoryEvent;

    public override bool IsPlayer => true;

    private PlayerData PlayerData => (PlayerData)characterData;
    private CharacterControllerPlayer PlayerController => (CharacterControllerPlayer)Controller;

    public PlayerCharacter() => Instance = this;

    private void Awake() => Initialize();

    protected override void Initialize()
    {
        SaveGameManager.Register(this);
        base.Initialize();
    }

    public void OnCaughtAllPokemons() => caughtAllPokemonsStoryEvent.TryInvoke();

    public Coroutine Defeat() => StartCoroutine(DefeatCoroutine());
    private IEnumerator DefeatCoroutine()
    {
        yield return DialogBox.Instance.DrawText($"Tja... ab ins Poke-Center. Schei?e gelaufen...", DialogBoxContinueMode.User, closeAfterFinish: true);
        PlayerData.Instance.HealAllPokemons();
        transform.position = PlayerData.lastPokeCenterEntrance.position;
        Movement.LookInDirection(Direction.Down);
    }

    public void EnterEntranceTreshhold(Door entrance)
    {
        PlayerController.EnterEntranceTrehshold(entrance);
    }

    public void LeaveEntranceTrehshold(Door entrance)
    {
        PlayerController.LeaveEntranceTrehshold(entrance);
    }

    public void TravelToEntrance(Door entrance) => Teleport(entrance.spawnPosition, -new GridVector(entrance.directionTriggerToEntrance));
    public void Teleport(Vector3 position, Direction direction) => Teleport(position, new GridVector(direction));
    public void Teleport(Vector3 position, GridVector direction)
    {
        transform.position = position;
        Movement.LookInDirection(direction);
    }

    public string GetKey()
        => $"{GetType()}";

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();

        //json.Add("name", playerData.name);
        json.Add("money", PlayerData.money);
        json.Add("position", transform.position);
        json.Add("direction", (Vector3)Movement.CurrentDirectionVector);
        json.Add("items", PlayerData.ItemsToJSON());
        json.Add("pokemons", PlayerData.PokemonsToJSON());
        json.Add("pokemonsInBox", PlayerData.PokemonsInBoxToJSON());
        json.Add("pokemonsSeen", PlayerData.SeenPokemonsToJSON());
        //json.Add("lastPokeCenterEntrance", );
        //json.Add("lastPokeCenterEntrance", );

        return json;
    }

    public void LoadFromJSON(JSONObject json)
    {
        JSONNode jsonData = json[GetKey()];
        //playerData.name = jsonData["name"];
        PlayerData.money = jsonData["money"];
        transform.position = jsonData["position"];
        Movement.LookInDirection(new GridVector(jsonData["direction"]));
        PlayerData.LoadItemsFromJSON((JSONArray)jsonData["items"]);
        PlayerData.LoadPokemonsFromJSON((JSONArray)jsonData["pokemons"]);
        PlayerData.LoadPokemonsInBoxFromJSON((JSONArray)jsonData["pokemonsInBox"]);
        PlayerData.LoadSeenPokemonsFromJSON((JSONArray)jsonData["pokemonsSeen"]);
    }

    public override void LoadDefault()
    {
        PlayerData.LoadDefault();
        base.LoadDefault();
    }
}
