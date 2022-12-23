using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] CharacterControllerBase controller;
    [SerializeField] CharacterMovement movement;
    [SerializeField] CharacterAnimator animator;

    public CharacterControllerBase Controller => controller;
    public CharacterMovement Movement => movement;
    public CharacterAnimator Animator => animator;
    public GridVector position => new GridVector(transform.position);
    public CharacterData characterData => controller.CharacterData;
    public Pokemon[] pokemons => controller.CharacterData is null ? new Pokemon[0] : controller.CharacterData.pokemons.ToArray();
    public Vector3 startPosition { get; private set; }
    public virtual bool IsPlayer => false;

    private void Awake() => Initialize();
    protected virtual void Initialize()
    {
        foreach (Pokemon pokemon in pokemons)
            pokemon.Initialize(characterData);

        startPosition = transform.position;

        controller.Initialize();
    }

    public virtual void LoadDefault()
    {
        foreach (Pokemon pokemon in pokemons)
            pokemon.LoadDefault();
    }

    public void Reset()
    {
        movement.LookInStartDirection();
        transform.position = startPosition;
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
}
