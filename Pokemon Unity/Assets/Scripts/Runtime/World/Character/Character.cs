using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Character follower;
    [field: SerializeField] public CharacterControllerBase Controller { get; private set; }
    [field: SerializeField] public CharacterMovement Movement { get; private set; }
    [field: SerializeField] public CharacterAnimator Animator { get; private set; }

    public GridVector Position => new GridVector(transform.position);
    public CharacterData CharacterData => Controller.CharacterData;
    public Pokemon[] Pokemons => Controller.CharacterData is null ? new Pokemon[0] : Controller.CharacterData.pokemons.ToArray();
    public Vector3 StartPosition { get; private set; }
    public virtual bool IsPlayer => false;

    private int layer;

    protected virtual void Awake()
    {
        foreach (Pokemon pokemon in Pokemons)
            pokemon.Initialize(CharacterData);

        StartPosition = transform.position;

        Controller.Initialize();
    }

    private void Start() => AddFollower(follower);

    public virtual void LoadDefault()
    {
        foreach (Pokemon pokemon in Pokemons)
            pokemon.LoadDefault();
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
}
