using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public abstract class Character : MonoBehaviour, ISavable
{
    [field: SerializeField] public CharacterControllerBase Controller { get; private set; }
    [field: SerializeField] public CharacterMovement Movement { get; private set; }
    [field: SerializeField] public CharacterAnimator Animator { get; private set; }
    [field: SerializeField] public BoxCollider Collider { get; private set; }
    [SerializeField] private Character follower;

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

    public void AddFollower(Character follower)
    {
        if (follower is null)
            return;

        follower.layer = transform.gameObject.layer;
        LayerManager.SetLayerRecursively(follower.gameObject, LayerManager.FollowerLayer);
        Movement.AddFollower(follower.Movement);
    }

    public void Unfollow()
    {
        LayerManager.SetLayerRecursively(follower.gameObject, follower.layer);
        Movement.Unfollow();
        follower = null;
    }

    public abstract string GetKey();

    public virtual JSONNode ToJSON()
    {
        JSONNode json = new JSONObject();

        var hasFollower = follower is not null;
        json.Add("hasFollower", hasFollower);
        if (hasFollower)
        {
            json.Add("followerPosition", follower.transform.position);
            json.Add("followerKey", follower.GetKey());
        }

        return json;
    }

    public virtual void LoadFromJSON(JSONNode json)
    {
        if (json["hasFollower"].AsBool || follower is not null)
        {
            if (follower is null)
                follower = SaveGameManager.GetSavable(json["followerKey"]) as Character;
            follower.transform.position = json["followerPosition"];
            AddFollower(follower);
        }
    }

    public virtual void LoadDefault()
    {
        PokemonsLoadDefault();
        if (follower is not null)
            AddFollower(follower);
    }

    protected void PokemonsLoadDefault()
    {
        foreach (Pokemon pokemon in Pokemons)
            pokemon.LoadDefault();
    }
}
