using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Character follower;
    [SerializeField] private CharacterControllerBase controller;
    [SerializeField] private CharacterMovement movement;
    [SerializeField] private CharacterAnimator animator;

    public CharacterControllerBase Controller => controller;
    public CharacterMovement Movement => movement;
    public CharacterAnimator Animator => animator;
    public GridVector Position => new GridVector(transform.position);
    public CharacterData CharacterData => controller.CharacterData;
    public Pokemon[] Pokemons => controller.CharacterData is null ? new Pokemon[0] : controller.CharacterData.pokemons.ToArray();
    public Vector3 StartPosition { get; private set; }
    public virtual bool IsPlayer => false;

    private int layer;

    protected virtual void Awake()
    {
        foreach (Pokemon pokemon in Pokemons)
            pokemon.Initialize(CharacterData);

        StartPosition = transform.position;

        controller.Initialize();
    }

    private void Start() => follower?.Follow(this);

    public virtual void LoadDefault()
    {
        foreach (Pokemon pokemon in Pokemons)
            pokemon.LoadDefault();
    }

    public void Reset()
    {
        movement.LookInStartDirection();
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
        => RaycastForward(movement.CurrentDirectionVector, layerMask);

    public void TryInteract()
    {
        RaycastHit hitInfo;
        if (RaycastForward(movement.CurrentDirectionVector, LayerManager.Instance.InteractableLayerMask, out hitInfo))
            hitInfo.collider.gameObject.GetComponent<IInteractable>()?.Interact(this); ;
    }

    public void Follow(Character target)
    {
        layer = transform.gameObject.layer;
        LayerManager.SetLayerRecursively(gameObject, LayerManager.FollowerLayer);
        movement.Follow(target.movement);
    }

    public void Unfollow()
    {
        LayerManager.SetLayerRecursively(gameObject, layer);
        movement.Unfollow();
    }
}
