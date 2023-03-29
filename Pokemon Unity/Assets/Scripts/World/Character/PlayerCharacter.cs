using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class PlayerCharacter : Character, IPlayerCharacter, ISavable
{
    public static PlayerCharacter Instance { get; private set; }
    
    public override bool IsPlayer => true;

    private PlayerData playerData => (PlayerData)characterData;
    private CharacterControllerPlayer playerController => (CharacterControllerPlayer)Controller;

    public PlayerCharacter()
    {
        Instance = this;
        Services.Register(this as IPlayerCharacter);
    }

    private void Awake() => Initialize();

    protected override void Initialize()
    {
        base.Initialize();
        Services.Get<ISaveGameManager>().Register(this);
    }

    public Coroutine Defeat() => StartCoroutine(DefeatCoroutine());
    private IEnumerator DefeatCoroutine()
    {
        yield return Services.Get<IDialogBox>().DrawText($"Tja... ab ins Poke-Center. Schei?e gelaufen...", DialogBoxContinueMode.User, closeAfterFinish: true);
        PlayerData.Instance.HealAllPokemons();
        transform.position = playerData.lastPokeCenterEntrance.position;
        Movement.LookInDirection(Direction.Down);
    }

    public void EnterEntranceTreshhold(Door entrance)
    {
        playerController.EnterEntranceTrehshold(entrance);
    }

    public void LeaveEntranceTrehshold(Door entrance)
    {
        playerController.LeaveEntranceTrehshold(entrance);
    }

    public void TravelToEntrance(Door entrance)
    {
        transform.position = entrance.spawnPosition;
        Movement.LookInDirection(-new GridVector(entrance.directionTriggerToEntrance));
    }

    public string GetKey()
        => $"{GetType()}";

    public JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();

        //json.Add("name", playerData.name);
        json.Add("money", playerData.money);
        json.Add("position", transform.position);
        json.Add("direction", (Vector3)Movement.CurrentDirectionVector);
        json.Add("items", playerData.ItemsToJSON());
        json.Add("pokemons", playerData.PokemonsToJSON());
        json.Add("pokemonsInBox", playerData.PokemonsInBoxToJSON());
        json.Add("pokemonsSeen", playerData.SeenPokemonsToJSON());
        //json.Add("lastPokeCenterEntrance", );
        //json.Add("lastPokeCenterEntrance", );

        return json;
    }

    public void LoadFromJSON(JSONObject json)
    {
        JSONNode jsonData = json[GetKey()];
        //playerData.name = jsonData["name"];
        playerData.money = jsonData["money"];
        transform.position = jsonData["position"];
        Movement.LookInDirection(new GridVector(jsonData["direction"]));
        playerData.LoadItemsFromJSON((JSONArray)jsonData["items"]);
        playerData.LoadPokemonsFromJSON((JSONArray)jsonData["pokemons"]);
        playerData.LoadPokemonsInBoxFromJSON((JSONArray)jsonData["pokemonsInBox"]);
        playerData.LoadSeenPokemonsFromJSON((JSONArray)jsonData["pokemonsSeen"]);
    }

    public override void LoadDefault()
    {
        playerData.LoadDefault();
        base.LoadDefault();
    }
}
