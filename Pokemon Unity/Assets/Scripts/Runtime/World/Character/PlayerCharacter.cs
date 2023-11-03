using System.Collections;
using UnityEngine;
using SimpleJSON;
using System.Linq;
using System;

public class PlayerCharacter : Character
{
    public static PlayerCharacter Instance { get; private set; }

    [SerializeField] private StoryEvent caughtAllPokemonsStoryEvent;
    [SerializeField] private PokemonData entonData;

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

    public override void LoadFromJSON(JSONNode json)
    {
        PlayerData.money = json["money"];
        transform.position = json["position"];
        Movement.LookInDirection(new GridVector(json["direction"]));
        PlayerData.LoadItemsFromJSON((JSONArray)json["items"]);
        PlayerData.LoadPokemonsFromJSON((JSONArray)json["pokemons"]);
        PlayerData.LoadPokemonsInBoxFromJSON((JSONArray)json["pokemonsInBox"]);
        PlayerData.LoadSeenPokemonsFromJSON((JSONArray)json["pokemonsSeen"]);
        base.LoadFromJSON(json);
    }

    public override void LoadDefault()
    {
        PlayerData.LoadDefault();
        base.LoadDefault();

        // Does player have Tessa`s Enton and is it still alive?
        DebugExtensions.DebugExtension.Log(PlayerData.pokemons);
        var tessasEnton = PlayerData.pokemons.Where(x => x.data == entonData && x.id == 0 && !x.isFainted);
        if (tessasEnton.Count() == 0)
            return;

        // if so, place Enton follower NPC
        PlaceFollower(tessasEnton.First().data.characterPrefab);
    }

    private void PlaceFollower(GameObject prefab, Direction direction = Direction.Up)
    {
        var freeDirection = Movement.GetMovableDirection(direction);
        if (freeDirection == Direction.None)
            throw new Exception($"No free location at character {gameObject.name} found for follower NPC!");

        var follower = Instantiate(prefab).GetComponent<Character>();
        follower.transform.position = Position + new GridVector(freeDirection);
        AddFollower(follower);
    }
}
