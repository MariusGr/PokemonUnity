using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

public abstract class Character : MonoBehaviour, ISavable
{
    [field: SerializeField] public CharacterControllerBase Controller { get; private set; }
    [field: SerializeField] public CharacterMovement Movement { get; private set; }
    [field: SerializeField] public CharacterAnimator Animator { get; private set; }
    [field: SerializeField] public BoxCollider Collider { get; private set; }
    [SerializeField] private Character follower;
    [SerializeField] private int followerPokemonTeamIndex = -1;

    public abstract CharacterData Data { get; }

    public GridVector Position => new(transform.position);
    public Pokemon[] Pokemons => Data is null ? new Pokemon[0] : Data.pokemons.ToArray();
    public Vector3 StartPosition { get; private set; }
    public virtual bool IsPlayer => false;

    private int layer;

    protected virtual void Awake()
    {
        foreach (Pokemon pokemon in Pokemons)
            pokemon.Initialize(Data);

        StartPosition = transform.position;

        Controller.Initialize();
        SaveGameManager.Register(this);
    }

    public void Reset()
    {
        Movement.LookInStartDirection();
        transform.position = StartPosition;
    }

    public bool RaycastForward(Vector3 direction, LayerMask layerMask, out RaycastHit hitInfo, float maxDistance = .8f)
    {
        Debug.DrawLine(transform.position + Vector3.up * .5f, transform.position + Vector3.up * .5f + direction * maxDistance, Color.red, .2f);
        return Physics.Raycast(
                   origin: transform.position + Vector3.up * .5f,
                   direction: direction,
                   maxDistance: maxDistance,
                   layerMask: layerMask,
                   hitInfo: out hitInfo
                   );
    }

    public bool RaycastForward(Vector3 direction, LayerMask layerMask)
    {
        RaycastHit hitInfo;
        return RaycastForward(direction, layerMask, out hitInfo);
    }

    public bool RaycastForward(LayerMask layerMask)
        => RaycastForward(Movement.CurrentDirectionVector, layerMask);

    public void TryInteract()
    {
        RaycastHit hitInfo;
        if (RaycastForward(Movement.CurrentDirectionVector, LayerManager.Instance.InteractableLayerMask, out hitInfo))
            hitInfo.collider.gameObject.GetComponent<IInteractable>()?.Interact(this); ;
    }

    public void SetFollower(Character follower, bool characterIsFromTeam)
    {
        if (follower is null)
            return;

        follower.layer = transform.gameObject.layer;
        LayerManager.SetLayerRecursively(follower.gameObject, LayerManager.FollowerLayer);
        Movement.SetFollower(follower.Movement);
        this.follower = follower;

        followerPokemonTeamIndex = characterIsFromTeam ? followerPokemonTeamIndex : -1;
    }

    public void Unfollow()
    {
        LayerManager.SetLayerRecursively(follower.gameObject, follower.layer);
        Movement.Unfollow();
        follower = null;
    }

    protected void PlaceFollowerPokemon(int index, Direction direction = Direction.Up)
    {
        followerPokemonTeamIndex = index;
        PlaceFollower(Data.pokemons[followerPokemonTeamIndex].data.characterPrefab, direction);
    }

    private void PlaceFollower(JSONNode json)
    {
        PlaceFollower(Data.pokemons[json["teamIndex"]].data.characterPrefab, json["position"]);
        follower.LoadFromJSON(json["character"]);
    }

    protected void PlaceFollower(GameObject prefab, Direction direction = Direction.Up)
    {
        var freeDirection = Movement.GetMovableDirection(direction);
        if (freeDirection == Direction.None)
            throw new Exception($"No free location at character {gameObject.name} found for follower NPC!");

        PlaceFollower(prefab, transform.position + new GridVector(freeDirection));
        follower.Movement.PlaceOnGround();
    }

    private void PlaceFollower(GameObject prefab, Vector3 position)
    {
        var newFollower = Instantiate(prefab).GetComponent<Character>();
        newFollower.transform.position = position;
        SetFollower(newFollower, true);
    }

    public abstract string GetKey();

    public virtual JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();

        var hasFollower = follower is not null;
        json.Add("hasFollower", hasFollower);

        JSONNode followerJSON = null;

        if (hasFollower)
        {
            followerJSON = new JSONObject();
            followerJSON["character"] = follower.ToJSON();
            followerJSON["teamIndex"] = followerPokemonTeamIndex;
            followerJSON.Add("position", follower.transform.position);
            followerJSON.Add("key", follower.GetKey());
        }

        json.Add("follower", followerJSON);

        return json;
    }

    public virtual void LoadFromJSON(JSONNode json)
    {
        if (json["hasFollower"].AsBool)
        {
            JSONNode followerJSON = json["follower"];

            followerPokemonTeamIndex = followerJSON["teamIndex"];

            if (followerPokemonTeamIndex > -1)
                PlaceFollower(followerJSON);
            else
            {
                follower = SaveGameManager.GetSavable(followerJSON["key"]) as Character;
                SetFollower(follower, false);
            }
        }
    }

    public virtual void LoadDefault()
    {
        PokemonsLoadDefault();
        if (follower is not null)
            SetFollower(follower, false);
    }

    protected void PokemonsLoadDefault()
    {
        foreach (Pokemon pokemon in Pokemons)
            pokemon.LoadDefault();
    }
}
